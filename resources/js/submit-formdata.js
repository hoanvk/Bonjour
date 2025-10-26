import axios from "axios";
import Swal from "sweetalert2";

const submitFormData = (form, selectors = []) => {
	const formElement = document.getElementById(form);
	const url = formElement.action;
	const formData = new FormData();

	selectors.forEach((selector) => {
		const inputElement = formElement.querySelector(selector);
		inputElement.classList.remove("is-invalid");
		var feedbackElement =
			inputElement.parentElement.querySelector(".invalid-feedback");
		if (feedbackElement) {
			feedbackElement.innerText = "";
		}
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
			console.log("error :>> ", error.response.data);
			Swal.fire({
				icon: "error",
				title: "Oops...",
				text: error.message,
			}).then(() => {
				Object.keys(error.response.data).forEach((prop) => {
					var inputElement = formElement.querySelector(
						`input[name='${prop}']`
					);
					if (inputElement) {
						inputElement.classList.add("is-invalid");
						var feedbackElement =
							inputElement.parentElement.querySelector(
								".invalid-feedback"
							);
						if (feedbackElement) {
							const errors = Array.isArray(
								error.response.data[prop]
							)
								? error.response.data[prop]
								: [error.response.data[prop]];
							feedbackElement.innerText = errors[0];
						}
					}
				});
			});
		});
};

export default submitFormData;
