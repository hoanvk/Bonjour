import submitFormData from '../submit-formdata';

function create() {
    submitFormData('createPermissionForm', ["input[name='Name']"]);
}
function update() {
    submitFormData('editPermissionForm', [
        "input[name='id']",
        "input[name='Name']",
    ]);
}
function remove(id) {
    const formElement = document.getElementById('deletePermissionForm');
    const hiddenElement = formElement.querySelector('input[name="id"]');
    hiddenElement.value = id;
    submitFormData('deletePermissionForm', ["input[name='id']"]);
}
function edit(button) {
    var formElement = document.getElementById('editPermissionForm');
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    console.log(entity);
    ["input[name='id']", "input[name='Name']"].forEach((selector) => {
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
