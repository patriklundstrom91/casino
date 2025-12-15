import { useState } from "react";
import "./App.css";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import {
  SignedIn,
  SignedOut,
  RedirectToSignIn,
  SignIn,
} from "@clerk/clerk-react";
import { CasinoLobby } from "./components/CasinoLobby";
import { Dices, Sparkles } from "lucide-react";
import { SlotsGame } from "./components/games/SlotsGame";
import { Account } from "./components/Account";

function App() {
  return (
    <Router>
      <Routes>
        <Route
          path="/"
          element={
            <>
              <SignedIn>
                <CasinoLobby />
              </SignedIn>
              <SignedOut>
                <div className="min-h-screen flex items-center justify-center p-4">
                  <div className="absolute inset-0 overflow-hidden">
                    <div className="absolute top-20 left-20 w-72 h-72 bg-amber-500/20 rounded-full blur-3xl animate-pulse" />
                    <div
                      className="absolute bottom-20 right-20 w-96 h-96 bg-amber-500/20 rounded-full blur-3xl animate-pulse"
                      style={{ animationDelay: "1s" }}
                    />
                  </div>

                  <div className="relative w-full max-w-md">
                    {/* Logo and Title */}
                    <div className="text-center mb-8">
                      <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-amber-400 to-amber-600 rounded-full mb-4 shadow-xl shadow-amber-500/50">
                        <Dices className="w-10 h-10 text-white" />
                      </div>
                      <h1 className="text-3xl mb-2 mt-2 flex items-center justify-center gap-2">
                        Welcome To Casino
                      </h1>
                      <h2 className="text-lg mb-3 py-4 flex items-center justify-center gap-3">
                        <Sparkles className="w-8 h-8 text-amber-400" />
                        Where Legends Are Made
                        <Sparkles className="w-8 h-8 text-amber-400" />
                      </h2>
                      <p className="text-lg">Sign In To Enter The Casino</p>
                    </div>
                    <div className="flex items-center justify-center p-8">
                      <SignIn />
                    </div>
                    <p className="text-center mt-6">
                      🎰 Win Big • Play Safe • Have Fun 🎰
                    </p>
                  </div>
                </div>
              </SignedOut>
            </>
          }
        />
        <Route
          path="/slots"
          element={
            <SignedIn>
              <SlotsGame />
            </SignedIn>
          }
        />
        <Route path="/blackjack" element={<SignedIn></SignedIn>} />
        <Route path="/roulette" element={<SignedIn></SignedIn>} />
        <Route path="/poker" element={<SignedIn></SignedIn>} />
        <Route
          path="/account"
          element={
            <SignedIn>
              <Account />
            </SignedIn>
          }
        />
        <Route
          path="*"
          element={
            <SignedOut>
              <RedirectToSignIn />
            </SignedOut>
          }
        />
      </Routes>
    </Router>
  );
}

export default App;
