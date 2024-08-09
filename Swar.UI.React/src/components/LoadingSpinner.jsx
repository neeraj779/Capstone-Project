const LoadingSpinner = () => (
  <svg
    className="w-5 h-5 text-white animate-spin"
    viewBox="0 0 24 24"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <circle
      className="opacity-25"
      cx="12"
      cy="12"
      r="10"
      stroke="currentColor"
      strokeWidth="4"
    ></circle>
    <path
      className="opacity-75"
      fill="currentColor"
      d="M4 12a8 8 0 018-8V2a10 10 0 00-10 10h2z"
    ></path>
  </svg>
);

export default LoadingSpinner;
