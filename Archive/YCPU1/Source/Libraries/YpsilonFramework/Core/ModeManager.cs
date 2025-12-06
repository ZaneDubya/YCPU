using Ypsilon.Core.Input;
using Ypsilon.Core.Patterns.MVC;

namespace Ypsilon.Core {
    public class ModeManager {
        private AModel m_Model;
        private AModel m_QueuedModel;

        public AModel ActiveModel {
            get { return m_Model; }
            set {
                if (m_Model != null) {
                    m_Model.Dispose();
                    m_Model = null;
                }
                m_Model = value;
                m_Model?.Initialize();
            }
        }

        public AModel QueuedModel {
            get { return m_QueuedModel; }
            set {
                if (m_QueuedModel != null) {
                    m_QueuedModel.Dispose();
                    m_QueuedModel = null;
                }
                m_QueuedModel = value;
                m_QueuedModel?.Initialize();
            }
        }

        public void ActivateQueuedModel() {
            if (m_QueuedModel != null) {
                ActiveModel = QueuedModel;
                m_QueuedModel = null;
            }
        }

        public void Draw(float frameSeconds) {
            m_Model.GetView().Draw(frameSeconds);
        }

        public void Update(float totalSeconds, float frameSeconds) {
            if (m_QueuedModel != null) {
                ActivateQueuedModel();
            }
            InputManager input = ServiceRegistry.GetService<InputManager>();
            m_Model.Update(totalSeconds, frameSeconds);
            m_Model.GetController().Update(frameSeconds, input);
        }
    }
}