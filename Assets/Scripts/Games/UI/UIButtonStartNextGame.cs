public class UIButtonStartNextGame : UIButton {
	protected override void OnClick() {
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
	}
}
