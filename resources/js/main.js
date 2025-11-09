import "@fortawesome/fontawesome-free/css/all.css";
// Import our custom CSS
import "../scss/styles.scss";
import "../css/flash-on-update.css";
import "flatpickr/dist/flatpickr.css";
import "dropzone/dist/dropzone.css";
// Import all of Bootstrap's JS
import * as bootstrap from "bootstrap";
import flatpickr from "flatpickr";

import axios from "axios";
window.axios = axios;
import submitFormData from "./submit-formdata.js";
window.submitFormData = submitFormData;
import Swal from "sweetalert2";
window.Swal = Swal;

import uploadFiles from "./upload-files.js";
window.uploadFiles = uploadFiles;

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

window.createHttpPost = function (url, data, { onSuccess, onError } = {}) {
  axios
    .post(url, data)
    .then(function (response) {
      Swal.fire({
        title: "Good job!",
        text:
          typeof response.data === "string"
            ? response.data
            : "Operation completed successfully.",
        icon: "success",
      }).then(() => {
        if (onSuccess) {
          onSuccess(response);
        }
      });
    })
    .catch(function (error) {
      Swal.fire({
        icon: "error",
        title: "Oops...",
        text:
          Object.keys(error.response.data).length === 0
            ? error.message
            : error.response.data[Object.keys(error.response.data)[0]],
      }).then(() => {
        if (onError) {
          onError(error);
        }
      });
    });
};

window.createHttpGet = function (url, { onSuccess, onError } = {}) {
  axios
    .get(url)
    .then(function (response) {
      if (onSuccess) {
        onSuccess(response);
      }
    })
    .catch(function (error) {
      Swal.fire({
        icon: "error",
        title: "Oops...",
        text: error.message,
      }).then(() => {
        if (onError) {
          onError(error);
        }
      });
    });
};

window.ConfirmDialog = function (title, message, onConfirmed) {
  Swal.fire({
    title,
    text: message,
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, confirm it!",
  }).then((result) => {
    if (result.isConfirmed) {
      onConfirmed();
    }
  });
};

import "../images/en.png";
import "../images/vi.png";

window.setLanguage = (element) => {
  const formElement = document.getElementById("SetLanguageForm");
  const cultureElement = formElement.querySelector('input[name="culture"]');
  cultureElement.value = element.getAttribute("data-culture");
  formElement.submit();
};

import Contract from "./pages/Contract.js";
window.Contract = Contract;
