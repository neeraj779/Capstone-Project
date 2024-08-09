const MobileSearchBar = () => (
  <div className="md:hidden relative max-w-full mx-6 mb-6">
    <form id="mobile-search-form" className="relative">
      <input
        type="text"
        id="mobile-search-input"
        className="bg-gray-700 text-white placeholder-gray-400 rounded-full py-2 px-4 pl-10 w-full"
        placeholder="Search songs, artists..."
      />
      <svg
        className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"
        xmlns="http://www.w3.org/2000/svg"
        width="20"
        height="20"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="2"
          d="M11 4a7 7 0 100 14 7 7 0 000-14zM21 21l-4.35-4.35"
        />
      </svg>
    </form>
  </div>
);

export default MobileSearchBar;
