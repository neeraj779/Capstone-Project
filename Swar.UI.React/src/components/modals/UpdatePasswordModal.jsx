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
  Input,
} from "@nextui-org/react";
import { LockIcon } from "./LockIcon.jsx";
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
    .notOneOf(
      [ref("currentPassword")],
      "New password must be different from the current password"
    ),
  confirmPassword: string()
    .required("Please confirm your new password")
    .oneOf([ref("newPassword")], "Passwords do not match"),
});

// eslint-disable-next-line react/prop-types
export default function UpdatePasswordModal({ isOpen, onOpenChange }) {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setError,
  } = useForm({ resolver: yupResolver(schema) });
  const swarApiClient = useApiClient();

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
      placement="center"
      radius="lg"
      classNames={{
        body: "py-6",
        backdrop: "blur",
        base: "bg-[#1a202c] text-[#fff]",
        header: "border-b-[1px] border-[#292f46]",
        footer: "border-t-[1px] border-[#292f46]",
        closeButton: "hover:bg-white/5 active:bg-white/10",
      }}
    >
      <ModalContent>
        <ModalHeader className="flex flex-col gap-1">
          Update Password
        </ModalHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          <ModalBody>
            <Input
              autoFocus
              isRequired
              endContent={
                <LockIcon className="text-2xl text-default-400 pointer-events-none flex-shrink-0" />
              }
              label="Current Password"
              placeholder="Enter your current password"
              type="password"
              variant="bordered"
              {...register("currentPassword")}
              isInvalid={!!errors.currentPassword}
              errorMessage={errors.currentPassword?.message}
            />
            <Input
              isRequired
              endContent={
                <LockIcon className="text-2xl text-default-400 pointer-events-none flex-shrink-0" />
              }
              label="New Password"
              placeholder="Enter your new password"
              type="password"
              variant="bordered"
              {...register("newPassword")}
              isInvalid={!!errors.newPassword}
              errorMessage={errors.newPassword?.message}
            />
            <Input
              isRequired
              endContent={
                <LockIcon className="text-2xl text-default-400 pointer-events-none flex-shrink-0" />
              }
              label="Confirm New Password"
              placeholder="Confirm your new password"
              type="password"
              variant="bordered"
              {...register("confirmPassword")}
              isInvalid={!!errors.confirmPassword}
              errorMessage={errors.confirmPassword?.message}
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
              Update Password
            </Button>
          </ModalFooter>
        </form>
      </ModalContent>
    </Modal>
  );
}
