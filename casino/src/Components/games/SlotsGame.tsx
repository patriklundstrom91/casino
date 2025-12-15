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
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-0 left-1/4 w-64 h-64 sm:w-96 sm:h-96 bg-blue-500/10 rounded-full blur-3xl animate-pulse" />
        <div
          className="absolute bottom-0 right-1/4 w-64 h-64 sm:w-96 sm:h-96 bg-cyan-500/10 rounded-full blur-3xl animate-pulse"
          style={{ animationDelay: "1.5s" }}
        />
      </div>

      <div className="relative">
        <header className="bg-black/30 backdrop-blur-md border-b border-white/10 sticky top-0 z-50">
          <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-3 sm:py-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2 sm:gap-3 flex-1">
                <div
                  className="hover-3d cursor-pointer p-2 sm:p-3 rounded-xl transition-all hover:scale-105 flex-shrink-0"
                  onClick={() => navigate("/")}
                >
                  <figure className="max-w-12 rounded-2xl">
                    <div className="w-10 h-10 sm:w-12 sm:h-12 bg-gradient-to-br from-amber-400 to-amber-600 rounded-full flex items-center justify-center shadow-lg shadow-amber-500/50">
                      <Dices className="w-5 h-5 sm:w-6 sm:h-6 text-white" />
                    </div>
                  </figure>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                </div>
                <div className="hidden sm:block">
                  <h1 className="text-lg font-bold text-white">Casino</h1>
                  <p className="text-xs text-gray-400">Slots</p>
                </div>
              </div>

              <div className="flex items-center gap-2 sm:gap-4">
                <div className="flex items-center gap-1 sm:gap-2 px-2 sm:px-4 py-2 bg-white/10 rounded-full border border-white/20 hover:shadow min-w-[80px]">
                  <Coins className="w-4 h-4 sm:w-5 sm:h-5 text-amber-400" />
                  <span className="text-sm sm:text-base font-bold text-white truncate">
                    ${user?.balance?.toFixed(2) || "0.00"}
                  </span>
                </div>
                <UserButton afterSignOutUrl="/" />
              </div>
            </div>
          </div>
        </header>

        <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-12">
          <div className="text-center mb-8 sm:mb-12">
            <h1 className="mb-3 flex items-center justify-center gap-3 text-xl sm:text-2xl font-bold text-white truncate">
              Welcome {userName}
            </h1>
            <h1 className="text-5xl sm:text-6xl font-black text-center mb-12 bg-gradient-to-r from-yellow-400 via-emerald-400 to-purple-400 bg-clip-text text-transparent drop-shadow-2xl">
              🎰 SUPER SLOTS
            </h1>
          </div>

          <div className="mb-12 sm:mb-16 flex justify-center">
            <div className="grid grid-cols-3 gap-4 sm:gap-8 p-6 sm:p-12 bg-black/40 rounded-2xl sm:rounded-3xl backdrop-blur-xl border-4 border-white/20 shadow-2xl w-full max-w-md sm:max-w-4xl">
              {reels.map((symbol, i) => (
                <div
                  key={i}
                  className="w-full aspect-square max-w-[120px] sm:w-44 sm:h-44 flex items-center justify-center text-4xl sm:text-6xl rounded-2xl sm:rounded-3xl 
                    bg-gradient-to-br from-white/20 to-transparent border-4 border-white/40 
                    shadow-2xl backdrop-blur-md hover:scale-105 transition-all duration-300 flex-shrink-0"
                >
                  {symbolIcon(symbol)}
                </div>
              ))}
            </div>
          </div>

          <div className="max-w-md mx-auto mb-12 sm:mb-16">
            <div className="text-center mb-6 sm:mb-8">
              <label className="block text-xl sm:text-2xl font-semibold mb-4 sm:mb-6 text-emerald-300">
                Bet Amount
              </label>
              <div className="grid grid-cols-5 gap-2 sm:gap-3 bg-black/40 p-4 sm:p-6 rounded-2xl backdrop-blur-md border border-white/20 shadow-xl">
                {betOptions.map((betOption) => (
                  <button
                    key={betOption}
                    onClick={() => setSelectedBet(betOption)}
                    className={`px-3 py-4 sm:px-4 sm:py-3 rounded-xl font-bold text-sm sm:text-base transition-all touch-manipulation active:scale-95 ${
                      selectedBet === betOption
                        ? "bg-emerald-500 text-black shadow-emerald-500/50 shadow-lg scale-105"
                        : "bg-white/10 hover:bg-white/20 hover:scale-105 text-white/80"
                    }`}
                  >
                    ${betOption}
                  </button>
                ))}
              </div>
            </div>
            <div className="text-center text-lg sm:text-xl opacity-90 mb-8 sm:mb-12">
              Selected:{" "}
              <span className="text-2xl sm:text-3xl font-black text-emerald-400">
                ${selectedBet}
              </span>
            </div>
          </div>

          <div className="max-w-md mx-auto text-center mb-12 sm:mb-16">
            <button
              onClick={spin}
              disabled={isSpinning || !user || user.balance < selectedBet}
              className="w-full px-8 sm:px-16 py-10 sm:py-12 bg-gradient-to-r from-emerald-500 via-green-500 to-emerald-600
                text-2xl sm:text-3xl font-black rounded-3xl shadow-2xl hover:shadow-emerald-500/50
                transform hover:scale-[1.02] active:scale-[0.98] touch-manipulation
                disabled:opacity-50 disabled:cursor-not-wait disabled:transform-none
                transition-all duration-300 border-4 border-emerald-400/50 min-h-[70px]"
            >
              {isSpinning
                ? "🎰 SPINNING..."
                : user?.balance < selectedBet
                ? `Need $${(selectedBet - (user?.balance || 0)).toFixed(
                    0
                  )} more`
                : `SPIN $${selectedBet} 🎰`}
            </button>
          </div>
          <div className="text-center mb-12 sm:mb-16">
            <div className="grid grid-cols-2 gap-4 sm:gap-6 max-w-md mx-auto text-sm sm:text-lg opacity-90">
              <div className="flex flex-col items-center p-4 bg-white/5 rounded-xl col-span-2 sm:col-span-1">
                💎 Diamond = 50x
              </div>
              <div className="flex flex-col items-center p-4 bg-white/5 rounded-xl col-span-2 sm:col-span-1">
                7️⃣ Seven = 20x
              </div>
              <div className="flex flex-col items-center p-4 bg-white/5 rounded-xl col-span-2 sm:col-span-1">
                3 of the same = 5x
              </div>
            </div>
          </div>
          <div className="text-center">
            <button
              className="inline-flex items-center gap-2 px-6 py-3 bg-white/10 hover:bg-white/20 rounded-2xl border border-white/20 transition-all text-white font-semibold text-sm sm:text-base"
              onClick={() => navigate("/")}
            >
              <ChevronLeft className="w-5 h-5" />
              Lobby
            </button>
          </div>
        </main>
      </div>
    </div>
  );
}
