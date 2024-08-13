import PropTypes from "prop-types";
import NotFoundError from "./NotResultError";
import BadRequestError from "./BadRequestError";

const ErrorMessage = ({ statusCode }) => {
  switch (statusCode) {
    case 404:
      return <NotFoundError />;
    case 400:
      return <BadRequestError />;
    default:
      return <p className="mt-2 text-sm">Oops! Something went wrong.</p>;
  }
};

ErrorMessage.propTypes = {
  statusCode: PropTypes.number,
};

export default ErrorMessage;
