import { useState } from "react";
import { useApi } from "../../api/useApi";

interface BlackjackResult {
  clerkUserId: string;
  playerHand: number[];
  dealerVisible: number[];
  dealerHand?: number[];
  status: string;
  winAmount: number;
  newBalance: number;
}

export const BlackjackGame = () => {
  const { apiFetch } = useApi();
  const [game, setGame] = useState<BlackjackResult | null>(null);
  const [bet, setBet] = useState(100);
  const [loading, setLoading] = useState(false);

  const joinGame = async () => {
    setLoading(true);
    try {
      const res = await apiFetch("/games/blackjack/join", {
        method: "POST",
        body: JSON.stringify({
          bet,
        }),
      });
      setGame(res);
    } catch (error) {
      console.error("Join failed:", error);
    }
    setLoading(false);
  };

  const hit = async () => {
    if (!game?.clerkUserId) return;
    setLoading(true);
    const res = await apiFetch(`/games/blackjack/${game.clerkUserId}/hit`, {
      method: "POST",
    });
    setGame(res);
    setLoading(false);
  };

  const stand = async () => {
    if (!game?.clerkUserId) return;
    setLoading(true);
    const res = await apiFetch(`/games/blackjack/${game.clerkUserId}/stand`, {
      method: "POST",
    });
    setGame(res);
    setLoading(false);
  };

  const getHandValue = (hand: number[]): string => {
    let value = hand.reduce((sum, card) => sum + card, 0);
    let aces = hand.filter((c) => c === 11).length;
    while (value > 21 && aces > 0) {
      value -= 10;
      aces--;
    }
    return value.toString();
  };

  const getCardEmoji = (value: number): string => {
    const cards = {
      2: "2️⃣",
      3: "3️⃣",
      4: "4️⃣",
      5: "5️⃣",
      6: "6️⃣",
      7: "7️⃣",
      8: "8️⃣",
      9: "9️⃣",
      10: "🎴",
      11: "🅰️",
    };
    return cards[value as keyof typeof cards] || "🃏";
  };

  return (
    <div className="blackjack-container max-w-4xl mx-auto p-8 bg-gradient-to-br from-emerald-900 via-green-900 to-black rounded-3xl shadow-2xl border-4 border-yellow-500/30">
      <div className="bet-section mb-12 p-6 bg-black/50 rounded-2xl border-2 border-yellow-400/50 backdrop-blur-sm">
        <div className="flex items-center justify-center gap-6">
          <input
            type="number"
            value={bet}
            onChange={(e) => setBet(Math.max(10, +e.target.value))}
            className="w-28 p-4 text-2xl bg-gradient-to-r from-yellow-500 to-orange-500 text-black font-bold rounded-xl shadow-lg focus:outline-none focus:ring-4 ring-yellow-400/50 text-center"
            min="10"
          />
          <button
            onClick={joinGame}
            disabled={loading || !!game}
            className="px-12 py-6 bg-gradient-to-r from-yellow-500 via-orange-500 to-red-500 text-black font-bold text-xl rounded-2xl shadow-2xl hover:scale-105 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed border-4 border-yellow-300/50"
          >
            {loading ? "🃏 DEALING..." : `DEAL ${bet}kr`}
          </button>
          {game && (
            <button
              onClick={() => setGame(null)}
              className="px-8 py-4 bg-gradient-to-r from-gray-700 to-gray-800 text-white font-bold rounded-xl hover:bg-gray-600 transition-all shadow-lg"
            >
              New Game
            </button>
          )}
        </div>
      </div>

      {game && (
        <div className="game-table">
          <div className="player-hand mb-12">
            <div className="flex justify-center items-end h-32 mb-4">
              {game.playerHand.map((card, i) => (
                <div
                  key={i}
                  className="card mx-2 w-20 h-32 bg-gradient-to-b from-red-500 to-black rounded-lg shadow-2xl border-4 border-white/50 flex items-end p-2 transform hover:scale-110 transition-all duration-200"
                >
                  <span className="text-2xl font-bold drop-shadow-lg">
                    {getCardEmoji(card)}
                  </span>
                </div>
              ))}
            </div>
            <div className="text-center">
              <span className="text-3xl font-bold text-yellow-400 drop-shadow-lg">
                Value: {getHandValue(game.playerHand)}
              </span>
            </div>
          </div>

          <div className="dealer-hand mb-12">
            <h2 className="text-2xl font-bold text-white mb-4 text-center tracking-wider uppercase">
              Dealer
            </h2>
            <div className="flex justify-center items-end h-32">
              {game.dealerVisible?.map((card, i) => (
                <div
                  key={i}
                  className="card mx-2 w-20 h-32 bg-gradient-to-b from-blue-600 to-black rounded-lg shadow-2xl border-4 border-white/50 flex items-end p-2 transform hover:scale-110 transition-all duration-200"
                >
                  <span className="text-2xl font-bold drop-shadow-lg">
                    {getCardEmoji(card)}
                  </span>
                </div>
              ))}
              {game.dealerHand && game.status !== "InProgress" && (
                <>
                  {game.dealerHand.slice(1).map((card, i) => (
                    <div
                      key={`dealer-${i}`}
                      className="card mx-2 w-20 h-32 bg-gradient-to-b from-blue-600 to-black rounded-lg shadow-2xl border-4 border-white/50 flex items-end p-2 transform hover:scale-110 transition-all duration-200"
                    >
                      <span className="text-2xl font-bold drop-shadow-lg">
                        {getCardEmoji(card)}
                      </span>
                    </div>
                  ))}
                </>
              )}
            </div>
            {game.dealerHand && game.status !== "InProgress" && (
              <div className="text-center mt-4">
                <span className="text-3xl font-bold text-yellow-400 drop-shadow-lg">
                  Value: {getHandValue(game.dealerHand)}
                </span>
              </div>
            )}
          </div>

          {/* Game Controls */}
          <div className="controls flex justify-center gap-8 mb-12">
            <button
              onClick={hit}
              disabled={game.status !== "InProgress" || loading}
              className="px-12 py-6 bg-gradient-to-r from-red-600 to-red-800 text-white font-bold text-xl rounded-2xl shadow-2xl hover:from-red-500 hover:to-red-700 transform hover:scale-105 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed border-4 border-red-400/50 min-w-[120px]"
            >
              HIT 💥
            </button>
            <button
              onClick={stand}
              disabled={game.status !== "InProgress" || loading}
              className="px-12 py-6 bg-gradient-to-r from-green-600 to-emerald-700 text-white font-bold text-xl rounded-2xl shadow-2xl hover:from-green-500 hover:to-emerald-600 transform hover:scale-105 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed border-4 border-green-400/50 min-w-[120px]"
            >
              STAND 🛑
            </button>
          </div>

          {/* Result & Balance */}
          {game.status !== "InProgress" && (
            <div
              className={`result text-center p-8 rounded-2xl ${
                game.status === "PlayerWin"
                  ? "bg-gradient-to-r from-green-600 to-emerald-600 border-4 border-green-400"
                  : game.status === "PlayerBust"
                  ? "bg-gradient-to-r from-red-600 to-red-800 border-4 border-red-400"
                  : "bg-gradient-to-r from-yellow-600 to-orange-600 border-4 border-yellow-400"
              }`}
            >
              <h2 className="text-4xl font-black mb-4 drop-shadow-2xl">
                {game.status === "PlayerWin" && "🎉 BLACKJACK WIN! 🎉"}
                {game.status === "PlayerBust" && "💥 BUST! 💥"}
                {game.status === "PlayerLose" && "😵 YOU LOSE"}
                {game.status === "PlayerBlackjack" &&
                  "👑 NATURAL BLACKJACK! 👑"}
              </h2>
              <p className="text-2xl font-bold mb-2">
                Win:{" "}
                <span className="text-3xl">
                  {game.winAmount.toLocaleString()}kr
                </span>
              </p>
              <p className="text-xl">
                New Balance:{" "}
                <span className="text-3xl font-bold text-yellow-400">
                  {game.newBalance.toLocaleString()}kr
                </span>
              </p>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
