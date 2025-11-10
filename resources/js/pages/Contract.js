import submitFormData from '../submit-formdata';

function create() {
    submitFormData('createContractForm', [
        "input[name='Name']",
        "input[name='Customer']",
        "input[name='StartDate']",
        "input[name='EndDate']",
    ]);
}
function update() {
    submitFormData('editContractForm', [
        "input[name='id']",
        "input[name='Name']",
        "input[name='Customer']",
        "input[name='StartDate']",
        "input[name='EndDate']",
    ]);
}
function remove(id) {
    const formElement = document.getElementById('deleteContractForm');
    const hiddenElement = formElement.querySelector('input[name="id"]');
    hiddenElement.value = id;
    submitFormData('deleteContractForm', ["input[name='id']"]);
}

function edit(button) {
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    var formElement = document.getElementById('editContractForm');
    console.log(entity);
    [
        "input[name='id']",
        "input[name='Name']",
        "input[name='Customer']",
        "input[name='StartDate']",
        "input[name='EndDate']",
    ].forEach((selector) => {
        var inputElement = formElement.querySelector(selector);
        var prop = inputElement.getAttribute('name');
        prop = prop.charAt(0).toLocaleLowerCase() + prop.slice(1);
        inputElement.value = entity[prop];
    });
}

export default {
    create,
    edit,
    update,
    remove,
};
