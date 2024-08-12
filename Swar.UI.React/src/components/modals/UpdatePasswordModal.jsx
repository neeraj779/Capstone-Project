import { useState } from "react";
import PropTypes from "prop-types";
import { useForm } from "react-hook-form";
import { object, string, ref } from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
} from "@nextui-org/react";
import PasswordInput from "./PasswordInput";
import { toast } from "react-hot-toast";
import useApiClient from "../../hooks/useApiClient";

const schema = object().shape({
  currentPassword: string().required("Current password is required"),
  newPassword: string()
    .required("New password is required")
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d\W_]{8,}$/,
      "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, and a number."
    )
    .test(
      "not-same-as-current",
      "New password must be different from the current password",
      function (value) {
        return value !== this.parent.currentPassword;
      }
    ),
  confirmPassword: string()
    .required("Please confirm your new password")
    .oneOf([ref("newPassword")], "Passwords do not match"),
});

const UpdatePasswordModal = ({ isOpen, onOpenChange }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setError,
  } = useForm({
    resolver: yupResolver(schema),
    mode: "onChange",
    reValidateMode: "onChange",
  });
  const swarApiClient = useApiClient();
  const [visibility, setVisibility] = useState({
    currentPassword: false,
    newPassword: false,
    confirmPassword: false,
  });

  const toggleVisibility = (field) => {
    setVisibility((prevState) => ({
      ...prevState,
      [field]: !prevState[field],
    }));
  };

  const onSubmit = async (data) => {
    console.log(data);
    try {
      await swarApiClient.patch("/users/me/password", {
        oldPassword: data.currentPassword,
        newPassword: data.newPassword,
      });
      toast.success("Password updated successfully");
      reset();
      onOpenChange(false);
    } catch (error) {
      if (error.response?.status === 401) {
        console.error(error);
        setError("currentPassword", {
          type: "manual",
          message: "Incorrect current password. Please try again.",
        });
      } else {
        toast.error("Failed to update password");
      }
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      isDismissable={false}
      size="full"
      radius="lg"
      classNames={{
        body: "py-6",
        backdrop: "blur",
        base: "bg-gradient-to-br from-gray-800 to-gray-900 p-2 shadow-2xl text-white",
        header: "border-b-[1px] border-[#292f46]",
        footer: "border-t-[1px] border-[#292f46]",
        closeButton: "hover:bg-white/5 active:bg-white/10",
      }}
    >
      <ModalContent>
        <ModalHeader className="flex flex-col gap-1">
          Change Password
          <p className="text-sm text-default-400">
            Your password must be at least 8 characters and should include a
            combination of uppercase, lowercase, and numbers.
          </p>
        </ModalHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          <ModalBody>
            <PasswordInput
              label="Current Password"
              placeholder="Enter your current password"
              name="currentPassword"
              register={register}
              errors={errors}
              visibility={visibility.currentPassword}
              toggleVisibility={toggleVisibility}
            />
            <PasswordInput
              label="New Password"
              placeholder="Enter your new password"
              name="newPassword"
              register={register}
              errors={errors}
              visibility={visibility.newPassword}
              toggleVisibility={toggleVisibility}
            />
            <PasswordInput
              label="Confirm New Password"
              placeholder="Confirm your new password"
              name="confirmPassword"
              register={register}
              errors={errors}
              visibility={visibility.confirmPassword}
              toggleVisibility={toggleVisibility}
            />
          </ModalBody>
          <ModalFooter>
            <Button
              color="secondary"
              variant="flat"
              onPress={() => onOpenChange(false)}
            >
              Cancel
            </Button>
            <Button color="primary" type="submit">
              Change Password
            </Button>
          </ModalFooter>
        </form>
      </ModalContent>
    </Modal>
  );
};

UpdatePasswordModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onOpenChange: PropTypes.func.isRequired,
};

export default UpdatePasswordModal;
