import { useNavigate } from "react-router-dom";
import { useApi } from "../../api/useApi";
import { UserButton, useUser } from "@clerk/clerk-react";
import { toast } from "react-toastify";
import { Coins, ChevronLeft, Dices } from "lucide-react";
import { useState } from "react";
import CustomUserButton from "../CustomUserButton";
import { useApiUser } from "../../api/useApiUser";

interface SlotsResult {
  symbols: string[];
  winAmount: number;
  newBalance: number;
}

export function SlotsGame() {
  const navigate = useNavigate();
  const { apiFetch } = useApi();
  const { user: clerkUser } = useUser();
  const { user, refetch } = useApiUser();
  const [isSpinning, setIsSpinning] = useState(false);
  const [reels, setReels] = useState(["Cherry", "Lemon", "Bell"]);
  const [selectedBet, setSelectedBet] = useState(10);
  const userName =
    clerkUser?.firstName ||
    clerkUser?.fullName ||
    clerkUser?.primaryEmailAddress?.emailAddress.split("@")[0];
  const betOptions = [10, 25, 50, 100, 250];

  const symbols = ["Cherry", "Lemon", "Bell", "Seven", "Diamond", "Star"];

  const spin = async () => {
    if (isSpinning || !user || user.balance < selectedBet) {
      toast.error("Insufficient balance or spinning!");
      return;
    }

    setIsSpinning(true);
    toast.loading("Spinning...", { duration: 2500 });

    const animationInterval = setInterval(() => {
      setReels((prev) =>
        prev.map(() => symbols[Math.floor(Math.random() * symbols.length)])
      );
    }, 150);

    setTimeout(async () => {
      clearInterval(animationInterval);

      try {
        console.log("Clerk ID:" + clerkUser?.id);
        const result = await apiFetch("/games/slots/spin", {
          method: "POST",
          body: JSON.stringify({
            clerkUserId: clerkUser?.id,
            bet: selectedBet,
          }),
        });

        setReels(result.symbols);
        await refetch();

        toast.dismiss();

        if (result.winAmount > 0) {
          toast.success(`🎉 WINNER! +$${result.winAmount.toFixed(2)}`);
        } else {
          toast.error(`No win. -$${selectedBet}`);
        }
      } catch (error: any) {
        toast.error(error.message || "Spin failed");
      } finally {
        setIsSpinning(false);
      }
    }, 2500);
  };

  const symbolIcon = (symbol: string) => {
    const icons: { [key: string]: string } = {
      Cherry: "🍒",
      Lemon: "🍋",
      Bell: "🔔",
      Seven: "7️⃣",
      Diamond: "💎",
      Star: "⭐",
    };
    return icons[symbol] || symbol;
  };

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
                  <p>Slots</p>
                </div>
              </div>

              <div className="flex items-center gap-4">
                <div className="flex items-center gap-2 px-2 py-2 bg-white/10 rounded-full border border-white/20 hover:shadow">
                  <Coins className="w-5 h-5 text-amber-400" />
                  <span>${user?.balance?.toFixed(2) || "0.00"}</span>
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
            <h1 className="text-5xl font-black text-center mb-12 bg-gradient-to-r from-yellow-400 via-emerald-400 to-purple-400 bg-clip-text text-transparent drop-shadow-2xl">
              🎰 SUPER SLOTS
            </h1>

            {/* Reels */}
            <div className="max-w-4xl mx-auto mb-12">
              <div className="grid grid-cols-3 gap-8 p-12 bg-black/40 rounded-3xl backdrop-blur-xl border-4 border-white/20 shadow-2xl">
                {reels.map((symbol, i) => (
                  <div
                    key={i}
                    className="w-44 h-44 flex items-center justify-center text-6xl rounded-3xl 
                        bg-gradient-to-br from-white/20 to-transparent border-4 border-white/40 
                        shadow-2xl backdrop-blur-md hover:scale-105 transition-all duration-300"
                  >
                    {symbolIcon(symbol)}
                  </div>
                ))}
              </div>
            </div>

            {/* Bet Selector */}
            <div className="max-w-md mx-auto mb-12">
              <div className="text-center mb-6">
                <label className="block text-xl font-semibold mb-4 text-emerald-300">
                  Bet Amount
                </label>
                <div className="grid grid-cols-5 gap-2 bg-black/40 p-4 rounded-2xl backdrop-blur-md border border-white/20">
                  {betOptions.map((betOption) => (
                    <button
                      key={betOption}
                      onClick={() => setSelectedBet(betOption)}
                      className={`px-4 py-3 rounded-xl font-bold transition-all ${
                        selectedBet === betOption
                          ? "bg-emerald-500 text-black shadow-emerald-500/50 shadow-lg scale-105"
                          : "bg-white/10 hover:bg-white/20 hover:scale-105"
                      }`}
                    >
                      ${betOption}
                    </button>
                  ))}
                </div>
              </div>

              <div className="text-center text-lg opacity-90 mb-8">
                Selected:{" "}
                <span className="text-2xl font-bold text-emerald-400">
                  ${selectedBet}
                </span>
              </div>
            </div>

            {/* Spin Button */}
            <div className="max-w-md mx-auto text-center">
              <button
                onClick={spin}
                disabled={isSpinning || !user || user.balance < selectedBet}
                className="w-full px-16 py-12 bg-gradient-to-r from-emerald-500 via-green-500 to-emerald-600
                     text-3xl font-black rounded-3xl shadow-2xl hover:shadow-emerald-500/50
                     transform hover:scale-[1.02] active:scale-[0.98]
                     disabled:opacity-50 disabled:cursor-not-wait disabled:transform-none
                     transition-all duration-300 border-4 border-emerald-400/50"
              >
                {isSpinning
                  ? "🎰 SPINNING..."
                  : user?.balance < selectedBet
                  ? `Need $${(selectedBet - (user?.balance || 0)).toFixed(
                      0
                    )} more`
                  : `SPIN $${selectedBet} 🎰`}
              </button>

              <div className="mt-6 text-lg opacity-90 grid grid-cols-2 gap-4 text-sm">
                <div>💎 Diamond = 50x</div>
                <div>7️⃣ Seven = 20x</div>
              </div>
            </div>

            <button className="btn" onClick={() => navigate("/")}>
              ← Lobby
            </button>
          </div>
        </main>
      </div>
    </div>
  );
}
