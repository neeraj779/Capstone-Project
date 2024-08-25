import NoResultsSVG from "../../assets/img/no-results.svg";

const NotResultError = () => (
  <div className="flex flex-col items-center justify-center">
    <img
      src={NoResultsSVG}
      alt="no results"
      className="h-64 transition-transform duration-500 transform opacity-75 w-96 hover:scale-105"
    />
    <h2 className="mb-2 text-2xl font-bold tracking-wide text-gray-100 md:text-3xl">
      Hmm, no results found
    </h2>
    <p className="p-6 text-lg text-gray-400 md:text-xl">
      It seems we couldn&apos;t find what you were looking for. Try adjusting
      your search or check back later!
    </p>
  </div>
);

export default NotResultError;
