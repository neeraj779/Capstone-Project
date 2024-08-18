import { useAuth0 } from "@auth0/auth0-react";
import { Button, Image } from "@nextui-org/react";
import logo from "../assets/img/logo.png";

const Logout = () => {
  const { loginWithRedirect } = useAuth0();

  return (
    <div className="min-h-screen-minus-navbar flex items-center justify-center bg-gray-900 px-4">
      <div className="bg-gradient-to-br from-gray-800 to-gray-900 rounded-2xl shadow-2xl border border-gray-700 w-full max-w-md p-10 ">
        <div className="text-center mb-8">
          <div className="flex items-center justify-center">
            <Image
              isBlurred
              src={logo}
              width={250}
              alt="Swar Logo"
              className="mb-6"
            />
          </div>
          <h1 className="text-4xl font-extrabold text-white">
            You’re Logged Out
          </h1>
          <p className="text-gray-400 mt-4">
            Thanks for visiting Swar! Your session ended, but feel free to log
            back in anytime. <br /> See you soon! 😊
          </p>
        </div>
        <div className="text-center">
          <Button
            auto
            color="primary"
            className="w-full text-lg font-semibold py-3"
            onClick={loginWithRedirect}
          >
            Log Back In
          </Button>
        </div>
      </div>
    </div>
  );
};

export default Logout;
