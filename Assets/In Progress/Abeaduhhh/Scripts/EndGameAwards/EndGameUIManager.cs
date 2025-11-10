//using UnityEngine;
//using TMPro;

//public class EndGameUIManager : MonoBehaviour {
//    [Header("Player UI Prefab")]
//    [SerializeField] private GameObject playerStatPrefab;

//    [Header("Parent Container")]
//    [SerializeField] private Transform statsContainer;

//    private void Start() {
//        PopulateStats();
//    }

//    private void PopulateStats() {
//        for (int i = 0; i < EndGameData.playerCount; i++) {
//            GameObject playerUI = Instantiate(playerStatPrefab, statsContainer);
//            TMP_Text[] texts = playerUI.GetComponentsInChildren<TMP_Text>();

//            foreach (var text in texts) {
//                switch (text.name) {
//                    case "PlayerNameText":
//                        text.text = $"Player {i + 1}";
//                        break;
//                    case "IngredientsText":
//                        text.text = EndGameData.ingredientsHandled[i].ToString();
//                        break;
//                    case "PointsText":
//                        text.text = EndGameData.pointsGenerated[i].ToString();
//                        break;
//                    case "JointsText":
//                        text.text = EndGameData.jointsReconnected[i].ToString();
//                        break;
//                    case "ExplosionsText":
//                        text.text = EndGameData.explosionsReceived[i].ToString();
//                        break;
//                }
//            }
//        }
//    }
//}
