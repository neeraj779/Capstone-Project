import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { create } from "../services/CRUDService";
import useAuth from "../hooks/useAuth";
import logo from "../assets/img/logo.png";
import LoadingSpinner from "../components/LoadingSpinner";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const { updateAccessToken, updateRefreshToken } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setLoading(true);
    setError("");

    try {
      const response = await create("users/login", { email, password });
      updateAccessToken(response.accessToken);
      updateRefreshToken(response.refreshToken);
      navigate("/");
    } catch (error) {
      setError(error.message || "An error occurred. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen-minus-navbar flex items-center justify-center bg-gray-900 px-4">
      <div className="bg-gray-800 bg-opacity-90 p-10 rounded-lg shadow-lg w-full max-w-md">
        <div className="text-center mb-8">
          <img src={logo} alt="Swar Logo" className="h-12 mx-auto mb-4" />
          <h1 className="text-3xl font-bold text-white">Welcome to Swar</h1>
          <p className="text-gray-400">Please login to continue</p>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label
              htmlFor="email"
              className="block text-sm font-semibold text-gray-400 mb-2"
            >
              Email
            </label>
            <input
              type="email"
              id="email"
              className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="mb-6">
            <label
              htmlFor="password"
              className="block text-sm font-semibold text-gray-400 mb-2"
            >
              Password
            </label>
            <input
              type="password"
              id="password"
              className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              minLength="8"
            />
          </div>
          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full transition duration-300 flex items-center justify-center"
            disabled={loading}
          >
            {loading ? (
              <>
                <span className="mr-3">Logging in...</span>
                <LoadingSpinner />
              </>
            ) : (
              <span>Login</span>
            )}
          </button>
          {error && (
            <div className="mt-4 text-red-500 text-center" aria-live="polite">
              {error}
            </div>
          )}
        </form>
        <div className="mt-6 text-center">
          <p className="text-gray-400">
            Don&apos;t have an account?{" "}
            <Link to="/register" className="text-blue-500 hover:underline">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;
