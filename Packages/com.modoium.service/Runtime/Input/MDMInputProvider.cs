using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modoium.Service {
    public class MDMInput : BaseInput {
        public static MDMInput instance { get; private set; }

        private MDMInputProvider _inputProvider;

        public bool touchPressureSupported => false;

        public override bool touchSupported => true;
        public override int touchCount => _inputProvider?.touchCount ?? 0;
        public override Touch GetTouch(int index) => _inputProvider.GetTouch(index).Value;

        internal void Configure(MDMInputProvider inputProvider) {
            _inputProvider = inputProvider;
            
            instance = this;
        }
    }

    internal enum MDMInputDeviceID : byte {
        HeadTracker = 0,
        LeftHandTracker = 1,
        RightHandTracker = 2,
        XRController = 3,
        TouchScreen = 4
    }

    internal enum MDMTouchScreenControl : byte {
        TouchIndexStart = 0,
        ToucnIndexEnd = 9,
        TouchCount = 10
    }

    internal enum MDMTouchPhase : byte {
        Ended = 0,
        Cancelled,
        Stationary,
        Moved
    }

    internal class MDMInputProvider {
        private const float TimeToWaitForEventSystem = 1.0f;

        private MDMService _owner;
        private MDMDisplayRotation _displayRotation;
        private List<Touch> _touches = new List<Touch>();
        private Dictionary<int, Touch> _fingerTouchMap = new Dictionary<int, Touch>();
        private MDMInput _input;
        private float _remainingToCreateEventSystem = -1.0f;

        internal int touchCount => _touches.Count;

        public MDMInputProvider(MDMService owner, MDMDisplayRotation displayRotation) {
            _owner = owner;
            _displayRotation = displayRotation;
        }

        public void Update() {
            if (Application.isPlaying == false || ModoiumPlugin.isXR) { return; }

            updateInputFrame();

            updateLegacyInputManager();
            updateInputSystem();
        }

        private void updateInputFrame() {
            ModoiumPlugin.UpdateInputFrame();
            updateTouches();
        }

        public Touch? GetTouch(int index) {
            if (index < 0 || index >= _touches.Count) { return null; }

            return _touches[index];
        }

        private void updateTouches() {
            _touches.Clear();
            if (_owner.remoteViewConnected == false) { return; }

            var remoteInputDesc = _owner.remoteInputDesc;
            var contentSize = (Display.main.renderingWidth, Display.main.renderingHeight);

            for (byte control = 0; control < (byte)MDMTouchScreenControl.TouchCount; control++) {
                var touch = getTouch(control, contentSize, remoteInputDesc);
                if (touch == null) { continue; }

                _touches.Add(touch.Value);
            }
        }

        private Touch? getTouch(byte control, (int width, int height) contentSize, MDMInputDesc remoteInputDesc) {
            var touchScreen = (byte)MDMInputDeviceID.TouchScreen;

            if (ModoiumPlugin.IsInputActive(touchScreen, control) == false &&
                ModoiumPlugin.GetInputDeactivated(touchScreen, control) == false) { return null; }

            ModoiumPlugin.GetInputTouch2D(touchScreen, control, out var position, out var state);

            position = _displayRotation.TranslateTouchInputPos(position, contentSize, remoteInputDesc);

            var touch = new Touch() {
                fingerId = control,
                type = TouchType.Direct,
                deltaTime = Time.unscaledDeltaTime,
                position = position,
                radius = 1,
                radiusVariance = 0,
                pressure = 1,
                maximumPossiblePressure = 1,
                altitudeAngle = 0,
                azimuthAngle = 0
            };

            if (ModoiumPlugin.GetInputActivated(touchScreen, control)) {
                touch.phase = TouchPhase.Began;
                retainFingerTouch(touch);
            }
            else {
                if (state == (byte)MDMTouchPhase.Ended || state == (byte)MDMTouchPhase.Cancelled) {
                    touch = releaseFingerTouch(touch);
                }
                else {
                    touch = updateFingerTouch(touch);
                }

                switch ((MDMTouchPhase)state) {
                    case MDMTouchPhase.Ended:
                        touch.phase = TouchPhase.Ended;
                        break;
                    case MDMTouchPhase.Cancelled:
                        touch.phase = TouchPhase.Canceled;
                        break;
                    case MDMTouchPhase.Stationary:
                        touch.phase = TouchPhase.Stationary;
                        break;
                    case MDMTouchPhase.Moved:
                        touch.phase = TouchPhase.Moved;
                        break;
                }
            }
            return touch;
        }

        private void retainFingerTouch(Touch touch) {
            touch.rawPosition = touch.position;

            if (_fingerTouchMap.ContainsKey(touch.fingerId)) {
                _fingerTouchMap[touch.fingerId] = touch;
            }
            else {
                _fingerTouchMap.Add(touch.fingerId, touch);
            }
        }

        private Touch updateFingerTouch(Touch touch) {
            if (_fingerTouchMap.ContainsKey(touch.fingerId) == false) { return touch; }

            var prev = _fingerTouchMap[touch.fingerId];
            touch.deltaPosition = touch.position - prev.position;
            touch.rawPosition = prev.rawPosition;
            _fingerTouchMap[touch.fingerId] = touch;

            return touch;
        }

        private Touch releaseFingerTouch(Touch touch) {
            if (_fingerTouchMap.ContainsKey(touch.fingerId) == false) { return touch; }

            touch.rawPosition = _fingerTouchMap[touch.fingerId].rawPosition;
            _fingerTouchMap.Remove(touch.fingerId);

            return touch;
        }

        private void updateLegacyInputManager() {
            if (_input != null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { 
                if (_remainingToCreateEventSystem < 0) {
                    _remainingToCreateEventSystem = TimeToWaitForEventSystem;
                }

                _remainingToCreateEventSystem -= Time.unscaledDeltaTime;
                if (_remainingToCreateEventSystem >= 0) { return; }

                eventSystem = createEventSystem();
            }
            _remainingToCreateEventSystem = TimeToWaitForEventSystem;
            
            _input = eventSystem.gameObject.GetComponent<MDMInput>();
            if (_input == null) {
                _input = eventSystem.gameObject.AddComponent<MDMInput>();
                _input.hideFlags = HideFlags.HideAndDontSave;

                _input.Configure(this);

                foreach (var inputModule in eventSystem.GetComponents<BaseInputModule>()) {
                    inputModule.inputOverride = _input;
                }
            }
        }

        private EventSystem createEventSystem() {
            var go = new GameObject("EventSystem") {
                hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector
            };
            var eventSystem = go.AddComponent<Modoium.Service.EventSystem>();
            go.AddComponent<StandaloneInputModule>();

            return eventSystem;
        }

#if UNITY_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
        private MDMTouchScreen _touchscreen;

        private void updateInputSystem() {
            if (_touchscreen == null) {
                _touchscreen = createTouchscreen();
            }
            _touchscreen.EnqueueInputEvents();
        }

        private MDMTouchScreen createTouchscreen() {
            var go = new GameObject("ModoiumTouchscreen") {
                hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector
            };
            var touchscreen = go.AddComponent<MDMTouchScreen>();
            Object.DontDestroyOnLoad(go);
    
            touchscreen.Configure(this);        
            return touchscreen;
        }
#else
        private void updateInputSystem() {}
#endif
    }
}
