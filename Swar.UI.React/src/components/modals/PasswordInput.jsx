import PropTypes from "prop-types";
import { Input } from "@nextui-org/react";
import { EyeFilledIcon, EyeSlashFilledIcon } from "./EyeIcons";

const PasswordInput = ({
  label,
  placeholder,
  register,
  name,
  errors,
  visibility,
  toggleVisibility,
}) => {
  return (
    <Input
      isRequired
      endContent={
        <button
          className="focus:outline-none"
          type="button"
          onClick={() => toggleVisibility(name)}
          aria-label="toggle password visibility"
        >
          {visibility ? (
            <EyeSlashFilledIcon className="text-2xl text-default-400 pointer-events-none" />
          ) : (
            <EyeFilledIcon className="text-2xl text-default-400 pointer-events-none" />
          )}
        </button>
      }
      label={label}
      classNames={{
        inputWrapper: ["border-gray-600", "focus-within:!border-white"],
      }}
      placeholder={placeholder}
      type={visibility ? "text" : "password"}
      variant="bordered"
      {...register(name)}
      isInvalid={!!errors[name]}
      errorMessage={errors[name]?.message}
    />
  );
};

PasswordInput.propTypes = {
  label: PropTypes.string.isRequired,
  placeholder: PropTypes.string.isRequired,
  register: PropTypes.func.isRequired,
  name: PropTypes.string.isRequired,
  errors: PropTypes.object.isRequired,
  visibility: PropTypes.bool.isRequired,
  toggleVisibility: PropTypes.func.isRequired,
};

export default PasswordInput;
