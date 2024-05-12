using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Attack
{
    public class AttackButton : MonoBehaviour
    {
        [SerializeField] private AttackDirection direction;
        [SerializeField] private Button button;
        
        private InputBus _inputBus;

        [Inject]
        public void Construct(InputBus inputBus)
        {
            _inputBus = inputBus;
        }

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _inputBus.AttackClicked(direction);
        }
        
        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}