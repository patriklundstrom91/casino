import { useNavigate } from "react-router-dom";
import { useUser } from "@clerk/clerk-react";
import { Coins, Dices } from "lucide-react";
import { useEffect, useState } from "react";
import CustomUserButton from "./CustomUserButton";
import { useApi } from "../api/useApi";

interface GameSession {
  id: number;
  gameType: string;
  bet: number;
  winAmount: number;
  netProfit: number;
  result: string;
  playedAt: string;
}

interface CasinoTransaction {
  id: number;
  amount: number;
  type: string;
  gameType: string;
  createdAt: string;
}

interface AccountInfo {
  clerkUserId: string;
  userName: string;
  balance: number;
  gameSessions: GameSession[];
  casinoTransactions: CasinoTransaction[];
}

export function Account() {
  const navigate = useNavigate();
  const { user, isLoaded } = useUser();
  const [account, setAccount] = useState<AccountInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const apiFetch = useApi().apiFetch;
  const userName =
    user?.firstName ||
    user?.fullName ||
    user?.primaryEmailAddress?.emailAddress.split("@")[0];

  useEffect(() => {
    if (!isLoaded || !user) return;
    fetchAccount();
  }, [user, isLoaded]);

  const fetchAccount = async () => {
    try {
      const res = await apiFetch("/user/account", {
        method: "GET",
      });
      setAccount(res);
    } catch (error) {
      console.error("Failed to load account:", error);
    } finally {
      setLoading(false);
    }
  };
  if (!isLoaded)
    return (
      <div className="flex justify-center items-center h-64">Loading...</div>
    );
  if (!user) {
    navigate("/sign-in");
    return null;
  }
  return (
    <div className="min-h-screen">
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute top-0 left-1/4 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl animate-pulse" />
        <div
          className="absolute bottom-0 right-1/4 w-96 h-96 bg-cyan-500/10 rounded-full blur-3xl animate-pulse"
          style={{ animationDelay: "1.5s" }}
        />
      </div>

      <div className="relative">
        <header className="bg-black/30 backdrop-blur-md border-b border-white/10">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div
                  className="hover-3d cursor-pointer p-2 rounded-xl transition-all hover:scale-105"
                  onClick={() => navigate("/")}
                >
                  {/* content */}
                  <figure className="max-w-100 rounded-2xl">
                    <div className="w-12 h-12 bg-gradient-to-br from-amber-400 to-amber-600 rounded-full flex items-center justify-center shadow-lg shadow-amber-500/50">
                      <Dices className="w-6 h-6 text-white" />
                    </div>
                  </figure>
                  {/* 8 empty divs needed for the 3D effect */}
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                </div>
                <div>
                  <h1>Casino</h1>
                  <p>My Account</p>
                </div>
              </div>

              <div className="flex items-center gap-4">
                <div className="flex items-center gap-2 px-2 py-2 bg-white/10 rounded-full border border-white/20 hover:shadow">
                  <Coins className="w-5 h-5 text-amber-400" />
                  <span>${account?.balance?.toFixed(2) || "0.00"}</span>
                </div>

                <div className="flex items-center gap-3">
                  <div className="flex items-center gap-2 px-2 py-2 bg-white/10 rounded-full border border-white/20">
                    <CustomUserButton />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </header>

        <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <div className="text-center mb-12">
            <h1 className="mb-3 flex items-center justify-center gap-3">
              Welcome {userName}
            </h1>
            <div className="space-y-6 sm:grid sm:grid-cols-2 sm:gap-6 sm:space-y-0">
              {/* Game Sessions */}
              <div className="bg-white/10 backdrop-blur-xl rounded-2xl p-6 sm:p-8 border border-white/20 shadow-2xl">
                <h2 className="text-xl sm:text-2xl font-bold text-white mb-6 flex items-center gap-2 sm:gap-3">
                  <span className="text-2xl">🎰</span>
                  Game History (Last 20)
                </h2>

                {loading ? (
                  <div className="flex justify-center py-12">
                    <div className="animate-spin rounded-full h-10 w-10 sm:h-12 sm:w-12 border-b-2 border-blue-400"></div>
                  </div>
                ) : account?.gameSessions.length ? (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="bg-white/20 rounded-xl backdrop-blur-sm">
                          <th className="p-3 sm:p-4 text-left font-semibold text-white/90">
                            Game
                          </th>
                          <th className="p-3 sm:p-4 text-right font-semibold text-white/90 hidden sm:table-cell">
                            Bet
                          </th>
                          <th className="p-3 sm:p-4 text-right font-semibold text-white/90 hidden md:table-cell">
                            Win
                          </th>
                          <th className="p-3 sm:p-4 text-right font-semibold text-white/90 lg:table-cell">
                            Net
                          </th>
                          <th className="p-3 sm:p-4 text-right font-semibold text-white/90">
                            Time
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {account.gameSessions.map((session) => (
                          <tr
                            key={session.id}
                            className="hover:bg-white/20 transition-all border-b border-white/10 last:border-b-0"
                          >
                            <td className="p-3 sm:p-4 font-medium capitalize text-white text-sm">
                              {session.gameType}
                            </td>
                            <td className="p-3 sm:p-4 text-right font-mono text-gray-300 hidden sm:table-cell">
                              ${session.bet.toFixed(2)}
                            </td>
                            <td
                              className={`p-3 sm:p-4 text-right font-mono font-semibold hidden md:table-cell ${
                                session.winAmount > 0
                                  ? "text-green-400"
                                  : "text-gray-400"
                              }`}
                            >
                              ${session.winAmount.toFixed(2)}
                            </td>
                            <td
                              className={`p-3 sm:p-4 text-right font-mono font-bold lg:table-cell ${
                                session.netProfit >= 0
                                  ? "text-green-400"
                                  : "text-red-400"
                              }`}
                            >
                              ${session.netProfit.toFixed(2)}
                            </td>
                            <td className="p-3 sm:p-4 text-right text-gray-400 text-xs">
                              {new Date(session.playedAt).toLocaleTimeString(
                                "en-US",
                                {
                                  hour: "2-digit",
                                  minute: "2-digit",
                                }
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <div className="text-center py-12 text-gray-400">
                    <p className="text-lg">No games played yet. Try Slots!</p>
                  </div>
                )}
              </div>

              {/* Transactions */}
              <div className="bg-white/10 backdrop-blur-xl rounded-2xl p-6 sm:p-8 border border-white/20 shadow-2xl">
                <h2 className="text-xl sm:text-2xl font-bold text-white mb-6 flex items-center gap-2 sm:gap-3">
                  <span className="text-2xl">💰</span>
                  Transactions (Last 10)
                </h2>

                {loading ? (
                  <div className="flex justify-center py-12">
                    <div className="animate-spin rounded-full h-10 w-10 sm:h-12 sm:w-12 border-b-2 border-green-400"></div>
                  </div>
                ) : account?.casinoTransactions.length ? (
                  <div className="space-y-3">
                    {account.casinoTransactions.map((transaction) => (
                      <div
                        key={transaction.id}
                        className="flex items-center justify-between p-4 bg-white/10 rounded-xl hover:bg-white/20 transition-all"
                      >
                        <div className="flex items-center gap-3 flex-1 min-w-0">
                          <div
                            className={`w-10 h-10 rounded-xl flex items-center justify-center text-xs font-bold shrink-0 ${
                              transaction.type === "Deposit"
                                ? "bg-green-500/20 text-green-400 border-2 border-green-500/30"
                                : "bg-red-500/20 text-red-400 border-2 border-red-500/30"
                            }`}
                          >
                            {transaction.type === "Deposit" ? "↑" : "↓"}
                          </div>
                          <div className="min-w-0">
                            <p className="font-semibold text-white truncate text-sm">
                              {transaction.type}
                            </p>
                            <p className="text-xs text-gray-400 truncate">
                              {transaction.gameType || "Account"}
                            </p>
                          </div>
                        </div>
                        <div className="text-right shrink-0">
                          <p
                            className={`text-xl sm:text-2xl font-black ${
                              transaction.type === "Deposit"
                                ? "text-green-400"
                                : "text-red-400"
                            }`}
                          >
                            ${transaction.amount.toFixed(2)}
                          </p>
                          <p className="text-xs text-gray-500 hidden sm:block">
                            {new Date(transaction.createdAt).toLocaleDateString(
                              "en-US"
                            )}
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-12 text-gray-400">
                    <p className="text-lg">No transactions yet.</p>
                  </div>
                )}
              </div>
            </div>
            <button className="btn mt-4" onClick={() => navigate("/")}>
              ← Lobby
            </button>
          </div>
        </main>
      </div>
    </div>
  );
}
