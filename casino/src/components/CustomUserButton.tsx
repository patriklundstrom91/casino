import { UserButton } from "@clerk/clerk-react";
import { useNavigate } from "react-router-dom";
import { User } from "lucide-react";

const CustomUserButton = () => {
  const navigate = useNavigate();
  return (
    <UserButton>
      <UserButton.MenuItems>
        <UserButton.Action
          label="My Account"
          labelIcon={<User />}
          onClick={() => navigate("/account")}
        />
      </UserButton.MenuItems>
    </UserButton>
  );
};

export default CustomUserButton;
