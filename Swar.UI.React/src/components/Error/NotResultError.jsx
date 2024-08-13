import NoResultsSVG from "../../assets/img/no-results.svg";

const NotResultError = () => (
  <div className="flex flex-col items-center justify-center">
    <img
      src={NoResultsSVG}
      alt="no results"
      className="w-96 h-64 opacity-75 transform transition-transform duration-500 hover:scale-105"
    />
    <h2 className="text-2xl md:text-3xl font-bold text-gray-100 tracking-wide mb-2">
      Hmm, no results found
    </h2>
    <p className="text-lg md:text-xl text-gray-400 p-6">
      It seems we couldn&apos;t find what you were looking for. Try adjusting
      your search or check back later!
    </p>
  </div>
);

export default NotResultError;
