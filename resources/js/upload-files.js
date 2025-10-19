import Dropzone from "dropzone";
import Swal from "sweetalert2";

const uploadFiles = function ({
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

export default uploadFiles;
