import BadRequestSVG from "../../assets/img/error-bad-request.svg";

const BadRequestError = () => (
  <div className="flex flex-col items-center justify-center mt-6">
    <img
      src={BadRequestSVG}
      alt="bad request"
      className="w-96 h-64 opacity-75 transform transition-transform duration-500 hover:scale-105"
    />
    <h2 className="text-2xl md:text-3xl font-bold text-gray-100 tracking-wide mb-2">
      Oops! We hit a snag
    </h2>
    <p className="text-lg md:text-xl text-gray-400 p-6">
      It looks like there was a hiccup in your request. Don’t worry, it happens!
      Let’s give it another shot.
    </p>
  </div>
);

export default BadRequestError;
