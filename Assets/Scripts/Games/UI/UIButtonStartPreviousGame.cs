public class UIButtonStartPreviousGame : UIButton {
	protected override void OnClick() {
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Previous);
	}
}
