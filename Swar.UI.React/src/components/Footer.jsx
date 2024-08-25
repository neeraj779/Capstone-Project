import { Link } from "@nextui-org/react";
const Footer = () => {
  return (
    <footer>
      <div className="px-6 mx-auto text-center max-w-7xl">
        <p className="text-sm">
          Made with ❤️ by{" "}
          <a href="https://github.com/neeraj779" target="_blank">
            Neeraj
          </a>
        </p>
        <div className="ml-2">
          <Link
            isBlock
            showAnchorIcon
            href="https://github.com/neeraj779"
            target="_blank"
            color="primary"
            size="sm"
          >
            Github
          </Link>
          <Link
            isBlock
            showAnchorIcon
            href="https://instagram.com/neeraj779_"
            target="_blank"
            color="primary"
            size="sm"
          >
            Instagram
          </Link>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
