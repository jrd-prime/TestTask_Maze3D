using System;
using System.Threading.Tasks;
using Mirror;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.Client.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameplayUI : NetworkBehaviour
    {
        [SerializeField] private PlayerCharacter playerCharacter;


        private VisualElement _root;

        private readonly CompositeDisposable _disposables = new();
        private Label _scoreHead;
        private Label _scoreNum;


        protected override void OnValidate()
        {
            if (playerCharacter == null) throw new NullReferenceException(nameof(playerCharacter));
        }

        private async void Start()
        {
            Debug.LogWarning("GameplayUI started.");
            // if (!isClient) return;

            playerCharacter = FindFirstObjectByType<PlayerCharacter>();
            if (playerCharacter == null) await Task.Delay(1000); //TODO hack - wait for spawn playerCharacter
            playerCharacter = FindFirstObjectByType<PlayerCharacter>();


            Debug.LogWarning("awake GameplayUI on client  id = " + playerCharacter.netId);

            _root = GetComponent<UIDocument>().rootVisualElement;

            if (_root == null) throw new NullReferenceException(nameof(_root));

            _scoreHead = _root.Q<Label>("score-head");
            _scoreNum = _root.Q<Label>("score-num");

            if (_scoreHead == null) throw new NullReferenceException(nameof(_scoreHead));
            if (_scoreNum == null) throw new NullReferenceException(nameof(_scoreNum));

            _scoreHead.text = "Score".ToUpper();
            _scoreNum.text = "0";


            playerCharacter.Score.Subscribe(OnScoreChanged).AddTo(_disposables);
        }

        [Client]
        private void OnScoreChanged(int score)
        {
            Debug.LogWarning("score changed to " + score);
            if (score < 0) return;
            _scoreNum.text = score.ToString();
        }
    }
}
