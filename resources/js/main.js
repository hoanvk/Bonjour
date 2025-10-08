import "@fortawesome/fontawesome-free/css/all.css";
// Import our custom CSS
import "../scss/styles.scss";
import "flatpickr/dist/flatpickr.css";
import "dropzone/dist/dropzone.css";
// Import all of Bootstrap's JS
import * as bootstrap from "bootstrap";
import flatpickr from "flatpickr";

import axios from "axios";
window.submitFormData = function ({ url, data }) {
	const formData = new FormData();
	Object.keys(data).forEach((key) => {
		formData.append(key, data[key]);
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
			location.reload();
		})
		.catch((error) => {
			console.log("error :>> ", error);
		});
};
import Swal from "sweetalert2";
import Dropzone from "dropzone";
window.uploadFiles = function ({
	selector,
	options: { url, paramName, acceptedFiles },
}) {
	new Dropzone(selector, {
		url,
		method: "POST",
		autoProcessQueue: true,
		paramName,
		clickable: true,
		maxFilesize: 5, //in mb
		addRemoveLinks: true,
		acceptedFiles,
		dictDefaultMessage: "Upload your file here",
		init: function () {
			this.on("sending", function (file, xhr, formData) {
				console.log("sending file");
			});
			this.on("success", function (file, responseText) {
				console.log("great success");
			});
			this.on("addedfile", function (file) {
				console.log("file added");
				Swal.fire("Uploaded successful!");
			});
			this.on("error", function (file, errorMessage, xhr) {
				// 'file' object contains details about the file that caused the error
				// 'errorMessage' is the error message provided by Dropzone or your custom validation
				// 'xhr' is the XMLHttpRequest object if the error came from the server

				// Example: Display a custom alert for the error
				alert("Error uploading " + file.name + ": " + errorMessage);

				// Example: Remove the erroneous file preview
				this.removeFile(file);
			});
		},
	});
};

const signalR = require("@microsoft/signalr");
window.RealtimeHub = function ({ selector }) {
	const connection = new signalR.HubConnectionBuilder()
		.withUrl("/realtimehub")
		.build();
	connection.on("ReceiveNotification", (message) => {
		console.log("message :>> ", message);
	});
	connection.start().catch((err) => console.error(err));

	function sendNotification() {
		const message = document.getElementById(selector).value;
		connection
			.invoke("SendNotification", message)
			.catch((err) => console.error(err));
	}
};

const Menu = require("./menu.js");
window.addEventListener("DOMContentLoaded", (event) => {
	flatpickr(".date", {
		dateFormat: "Y-m-d",
	});
	Menu();
});
