using UnityEngine;

using EventInfo = AnalyticsAgent.EventInfo;

/// <summary>
/// 
/// класс для отслеживания статистики. содержит методы трекания специфичных для игры событий.
/// 
/// не стоит трекать события через какие-то другие классы.
/// перенаправляет данные в централизованный менеджер статистики, который в свою очередь рулит
/// сервисами, в которые нужно отправлять статистику в зависимости от целевой платформы.
/// 
/// если каких-то событий/параметров не хватает, то добавлять их стоит именно сюда, а не городить очередной велосипед.
/// 
/// </summary>
public class AnalyticsTracker : AbstractSingleton<AnalyticsTracker, AnalyticsTracker> {
	public enum InAppTypes {
		Clicked,
		Bought,
	}

	public enum RateUsTypes {
		ParentsScreen,
		DialogYes,
		DialogNo,
	}

	private static void Track(EventInfo eventInfo) {
		AnalyticsManager.Instance.TrackEvent(eventInfo);
	}

	public static void GameAppStart() {
		var prefs = "appfirststart";
		var isFirstStart = PlayerPrefs.GetInt(prefs, 1) > 0;
		if (isFirstStart) {
			Track(new EventInfo("session_first"));
			PlayerPrefs.SetInt(prefs, 0);
			PlayerPrefs.Save();
		} else {
			Track(new EventInfo("session_next").Add("number", PrefsCounter("session_number")));
		}
	}

	public static void MiniGameStart(GameTypes game) {
		Track(new EventInfo("mini_game_start").Add("id", GameTypeToIntId(game)));
	}

	public static void MiniGameWin(GameTypes game) {
		Track(new EventInfo("mini_game_win").Add("id", GameTypeToIntId(game)));
	}

	public static void InApp(string sku, InAppTypes inAppType) {
		Track(new EventInfo("in_app").Add("sku", sku).Add("type", (int)inAppType));
	}

	public static void RateUs(RateUsTypes rateUsType) {
		Track(new EventInfo("rate_us").Add("type", (int)rateUsType));
	}

	public static void MoreGamesClicked(string otherGameName) {
		Track(new EventInfo("more_games_clicked").Add("game", otherGameName));
	}

	private static int GameTypeToIntId(GameTypes game) {
		switch (game) {
		case GameTypes.MatchShape:
			return 1;
		case GameTypes.MatchColor:
			return 2;
		case GameTypes.FallShapes:
			return 3;
		}

		return 0;
	}

	private static int PrefsCounter(string prefs) {
		prefs += "anltcscntr_" + prefs;
		var result = PlayerPrefs.GetInt(prefs, 0);
		PlayerPrefs.SetInt(prefs, result + 1);
		PlayerPrefs.Save();
		return result;
	}
}
