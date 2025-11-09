import submitFormData from "../submit-formdata";

export function createContractHandler() {
  submitFormData("createContractForm", [
    "input[name='Name']",
    "input[name='Customer']",
    "input[name='StartDate']",
    "input[name='EndDate']",
  ]);
}
export function editContractHandler() {
  submitFormData("editContractForm", [
    "input[name='id']",
    "input[name='Name']",
    "input[name='Customer']",
    "input[name='StartDate']",
    "input[name='EndDate']",
  ]);
}
export function deleteContractHandler(id) {
  const formElement = document.getElementById("deleteContractForm");
  const hiddenElement = formElement.querySelector('input[name="id"]');
  hiddenElement.value = id;
  submitFormData("deleteContractForm", ["input[name='id']"]);
}

export function showContractHandler() {
  const modalElement = document.getElementById("editContractModal");
  modalElement.addEventListener("show.bs.modal", (event) => {
    var formElement = document.getElementById("editContractForm");
    // Button that triggered the modal
    const button = event.relatedTarget;
    // Extract info from data-bs-* attributes
    const entity = JSON.parse(button.getAttribute("data-bs-entity"));
    console.log(entity);
    [
      "input[name='id']",
      "input[name='Name']",
      "input[name='Customer']",
      "input[name='StartDate']",
      "input[name='EndDate']",
    ].forEach((selector) => {
      var inputElement = formElement.querySelector(selector);
      var prop = inputElement.getAttribute("name");
      prop = prop.charAt(0).toLocaleLowerCase() + prop.slice(1);
      inputElement.value = entity[prop];
    });
  });
}
