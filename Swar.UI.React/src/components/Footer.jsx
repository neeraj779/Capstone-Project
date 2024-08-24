import { Link } from "@nextui-org/react";
const Footer = () => {
  return (
    <footer>
      <div className="max-w-7xl mx-auto px-6 text-center">
        <p className="text-sm">
          Made with ❤️ by{" "}
          <a href="https://github.com/neeraj779" target="_blank">
            Neeraj
          </a>
        </p>
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
    </footer>
  );
};

export default Footer;
