import axios from "axios";
import Swal from "sweetalert2";

const submitFormData = (form, selectors = []) => {
	const formElement = document.getElementById(form);
	console.log(formElement);
	const url = formElement.action;
	const formData = new FormData();

	selectors.forEach((selector) => {
		const inputElement = formElement.querySelector(selector);
		console.log(inputElement);
		formData.append(inputElement.getAttribute("name"), inputElement.value);
	});
	const token = document.querySelector(
		'input[name="__RequestVerificationToken"]'
	).value;

	axios
		.post(url, formData, {
			headers: {
				RequestVerificationToken: token,
			},
		})
		.then((success) => {
			console.log("success :>> ", success);
			Swal.fire({
				icon: "success",
				title: "Success!",
				text: success.data,
			}).then(() => {
				location.reload();
			});
		})
		.catch((error) => {
			console.log("error :>> ", error);
			Swal.fire({
				icon: "error",
				title: "Oops...",
				text: error.message,
			});
		});
};

export default submitFormData;
