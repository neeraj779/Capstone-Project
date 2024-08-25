import BadRequestSVG from "../../assets/img/error-bad-request.svg";

const BadRequestError = () => (
  <div className="flex flex-col items-center justify-center mt-6">
    <img
      src={BadRequestSVG}
      alt="bad request"
      className="h-64 transition-transform duration-500 transform opacity-75 w-96 hover:scale-105"
    />
    <h2 className="mb-2 text-2xl font-bold tracking-wide text-gray-100 md:text-3xl">
      Oops! We hit a snag
    </h2>
    <p className="p-6 text-lg text-gray-400 md:text-xl">
      It looks like there was a hiccup in your request. Don’t worry, it happens!
      Let’s give it another shot.
    </p>
  </div>
);

export default BadRequestError;
