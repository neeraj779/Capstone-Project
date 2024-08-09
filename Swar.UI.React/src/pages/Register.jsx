import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { create } from "../services/CRUDService";
import logo from "../assets/img/logo.png";
import LoadingSpinner from "../components/LoadingSpinner";

const Register = () => {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    gender: "",
    password: "",
    confirmPassword: "",
  });
  const [status, setStatus] = useState({
    loading: false,
    error: "",
    success: "",
  });
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setStatus({ loading: true, error: "", success: "" });

    if (formData.password !== formData.confirmPassword) {
      setStatus({
        loading: false,
        error: "Passwords do not match.",
        success: "",
      });
      return;
    }

    try {
      await create("users/register", formData);
      setStatus({
        loading: false,
        error: "",
        success: "Account created successfully! Redirecting to login...",
      });
      setTimeout(() => navigate("/login"), 2000);
    } catch (error) {
      setStatus({
        loading: false,
        error: error.message || "An error occurred. Please try again.",
        success: "",
      });
    }
  };

  const formFields = [
    { id: "name", type: "text", placeholder: "Enter your Name", label: "Name" },
    {
      id: "email",
      type: "email",
      placeholder: "Enter your email",
      label: "Email",
    },
    {
      id: "gender",
      type: "select",
      label: "Gender",
      options: ["Male", "Female", "Other"],
    },
    {
      id: "password",
      type: "password",
      placeholder: "Enter your password",
      label: "Password",
      minLength: 8,
    },
    {
      id: "confirmPassword",
      type: "password",
      placeholder: "Confirm your password",
      label: "Confirm Password",
      minLength: 8,
    },
  ];

  return (
    <div className="min-h-screen-minus-navbar flex items-center justify-center bg-gray-900 px-4">
      <div className="bg-gray-800 bg-opacity-90 p-10 rounded-lg shadow-lg w-full max-w-md mt-8 mb-8 md:mt-12 md:mb-12">
        <div className="text-center mb-8">
          <img src={logo} alt="Swar Logo" className="h-12 mx-auto mb-4" />
          <h1 className="text-3xl font-bold text-white">Join Swar</h1>
          <p className="text-gray-400">Create an account to enjoy our music</p>
        </div>
        <form onSubmit={handleSubmit}>
          {formFields.map(
            ({ id, type, placeholder, label, options, minLength }) => (
              <div key={id} className="mb-4">
                <label
                  htmlFor={id}
                  className="block text-sm font-semibold text-gray-400 mb-2"
                >
                  {label}
                </label>
                {type === "select" ? (
                  <select
                    id={id}
                    className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                    value={formData[id]}
                    onChange={handleChange}
                    required
                  >
                    <option value="" disabled>
                      Select your gender
                    </option>
                    {options.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                ) : (
                  <input
                    type={type}
                    id={id}
                    className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder={placeholder}
                    value={formData[id]}
                    onChange={handleChange}
                    required
                    minLength={minLength}
                  />
                )}
              </div>
            )
          )}
          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full transition duration-300 flex items-center justify-center"
            disabled={status.loading}
          >
            {status.loading ? (
              <>
                <span className="mr-3">Registering...</span>
                <LoadingSpinner />
              </>
            ) : (
              <span>Register</span>
            )}
          </button>
          {status.error && (
            <div className="mt-4 text-red-500 text-center" aria-live="polite">
              {status.error}
            </div>
          )}
          {status.success && (
            <div className="mt-4 text-green-500 text-center" aria-live="polite">
              {status.success}
            </div>
          )}
        </form>
        <div className="mt-6 text-center">
          <p className="text-gray-400">
            Already have an account?{" "}
            <Link to="/login" className="text-blue-500 hover:underline">
              Login
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
