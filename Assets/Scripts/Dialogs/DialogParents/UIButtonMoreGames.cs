using System.Collections.Generic;

public class UIButtonMoreGames : UIButton {
	private enum OtherGames {
		Lamphead,
		DropHunt,
		BlitzBlotz,
		Count,
	}

	private class GameUrls {
		private string[] urls = null;
		
		public string Url {
			get {
#if UNITY_ANDROID
				return urls[0];
#elif UNITY_IOS
				return urls[1];
#elif UNITY_WSA
				return urls[2];
#else
				return "http://oriplay.com";
#endif
			}
		}

		public GameUrls(string ios, string android, string wsa) {
			urls = new string[3] {
				android,
				ios,
				wsa,
			};
		}
	}

	private Dictionary<OtherGames, GameUrls> urls = new Dictionary<OtherGames, GameUrls> {
		{ OtherGames.Lamphead, new GameUrls("1074030366", "com.oriplay.lamphead", "9nblggh5kpw4") },
		{ OtherGames.DropHunt, new GameUrls("1011809324", "com.OriplayGames.DropHunt", "drop-hunt/9wzdncrdt2h0") },
		{ OtherGames.BlitzBlotz, new GameUrls("1042324945", "com.oriplay.BlitzBlotz", "blitz-blotz/9wzdncrdt2gz") },
	};

	private static int counter = 0;

	protected override void OnClick() {
		var otherGame = null as GameUrls;
		var game = (OtherGames)(counter++ % (int)OtherGames.Count);
		urls.TryGetValue(game, out otherGame);
		if (otherGame != null) {
			AnalyticsTracker.MoreGamesClicked(game.ToString());
			GameCore.GoToAppPageInStore(otherGame.Url);
		}
	}
}
