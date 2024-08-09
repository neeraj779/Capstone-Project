import PropTypes from "prop-types";

const ErrorMessage = ({ message }) => (
  <div
    id="search-error-message"
    className="text-center p-6 rounded-lg bg-gray-600 bg-opacity-50 mb-4"
  >
    <p className="mt-2 text-sm">{message || "Oops! Something went wrong."}</p>
  </div>
);

ErrorMessage.propTypes = {
  message: PropTypes.string,
};

export default ErrorMessage;
