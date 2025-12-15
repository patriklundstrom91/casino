import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useAuth } from "@clerk/clerk-react";
import { toast } from "react-toastify";

interface UserDto {
  userName: string;
  balance: number;
  totalTransactions: number;
  hasClaimedWelcomeBonus: boolean;
}

interface UpdateBalanceDto {
  balance: number;
}

export function useApiUser() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  const userQuery = useQuery<UserDto>({
    queryKey: ["user"],
    queryFn: async () => {
      const token = await getToken({ template: "Casino-JWT" });
      console.log("token ?=?)?=", token);
      const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5136";
      const res = await fetch(`${API_URL}/user`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      console.log("🌐 Fetching /user, status:", res.status);
      if (!res.ok) throw new Error("Failed to fetch user");
      return res.json();
    },
    staleTime: 30_000,
  });

  const updateBalance = useMutation({
    mutationFn: async ({
      amount,
      gameType,
    }: {
      amount: number;
      gameType: string;
    }) => {
      const token = await getToken({ template: "Casino-JWT" });
      const res = await fetch("/user/balance", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ amount, gameType }),
      });
      if (!res.ok) throw new Error("Failed to update balance");
      return res.json() as Promise<UpdateBalanceDto>;
    },
    onSuccess: (data) => {
      // Uppdatera cache automatiskt
      queryClient.setQueryData(["user"], (old: any) => ({
        ...old,
        balance: data.balance,
      }));
      toast.success(`Balance updated! $${data.balance.toFixed(2)}`);
    },
    onError: (error) => {
      toast.error("Balance update failed");
      console.error(error);
    },
  });

  return {
    user: userQuery.data,
    isLoading: userQuery.isLoading,
    error: userQuery.error,
    refetch: userQuery.refetch,
    updateBalance: updateBalance.mutate,
    isUpdating: updateBalance.isPending,
  };
}
