import { useNavigate } from "react-router-dom";
import { UserButton, useUser } from "@clerk/clerk-react";
import { toast } from "react-toastify";
import {
  Coins,
  Sparkles,
  CircleDot,
  Spade,
  Cherry,
  User,
  Dices,
} from "lucide-react";
import { useState } from "react";

export function Profile() {
  const navigate = useNavigate();
  const { user } = useUser();
  const userName =
    user?.firstName ||
    user?.fullName ||
    user?.primaryEmailAddress?.emailAddress.split("@")[0];

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
                <div className="hover-3d">
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
                  <p>Profile</p>
                </div>
              </div>

              <div className="flex items-center gap-4">
                <div className="flex items-center gap-2 px-2 py-2 bg-white/10 rounded-full border border-white/20 hover:shadow">
                  <Coins className="w-5 h-5 text-amber-400" />
                  <span>$10,000</span>
                </div>

                <div className="flex items-center gap-3">
                  <div className="flex items-center gap-2 px-2 py-2 bg-white/10 rounded-full border border-white/20">
                    <UserButton />
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
            <p className="py-2">
              Experience the thrill of world-class casino games
            </p>
            <h2 className="mb-3 py-4 flex items-center justify-center gap-3">
              <Sparkles className="w-8 h-8 text-amber-400" />
              Choose Your Game
              <Sparkles className="w-8 h-8 text-amber-400" />
            </h2>
          </div>

          {/* Games Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {games.map((game) => {
              const Icon = game.icon;
              return (
                <button
                  key={game.id}
                  onClick={() => navigate(game.path)}
                  className="group relative overflow-hidden bg-white/10 backdrop-blur-md rounded-2xl border border-white/20 p-8 hover:scale-105 transition-all duration-300 hover:shadow-2xl hover:border-white/40"
                >
                  {/* Background gradient on hover */}
                  <div
                    className={`absolute inset-0 bg-gradient-to-br ${game.color} opacity-0 group-hover:opacity-20 transition-opacity`}
                  />

                  {/* Icon background */}
                  <div
                    className={`w-20 h-20 ${game.bgColor} rounded-full flex items-center justify-center mx-auto mb-4 group-hover:scale-110 transition-transform`}
                  >
                    <Icon className="w-10 h-10 text-white" />
                  </div>

                  <h3 className="text-white mb-2">{game.name}</h3>
                  <p className="text-purple-200">{game.description}</p>

                  {/* Play button on hover */}
                  <div className="mt-4 transition-opacity">
                    <span
                      className={`inline-block px-6 py-2 bg-gradient-to-r ${game.color} text-white rounded-lg shadow-lg`}
                    >
                      Play Now
                    </span>
                  </div>
                </button>
              );
            })}
          </div>
        </main>
      </div>
    </div>
  );
}
