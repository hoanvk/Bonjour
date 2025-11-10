import submitFormData from '../submit-formdata';

function beforeSubmit(formId) {
    const formElement = document.getElementById(formId);
    var permissionElements = formElement.querySelectorAll(
        "input[name='Permission']"
    );
    var permissions = [];
    permissionElements.forEach((permissionElement) => {
        var action = '';
        var parentElement = permissionElement.parentElement;
        var readCheckElement =
            parentElement.querySelector("input[name='Read']");
        action += readCheckElement && readCheckElement.checked ? 'r' : '-';
        var writeCheckElement = parentElement.querySelector(
            "input[name='Write']"
        );
        action += writeCheckElement && writeCheckElement.checked ? 'w' : '-';
        var createCheckElement = parentElement.querySelector(
            "input[name='Create']"
        );
        action += createCheckElement && createCheckElement.checked ? 'c' : '-';
        permissions.push({
            action,
            permissionId: permissionElement.value,
        });
    });
    var roleHasPermissionElement = formElement.querySelector(
        "input[name='Permissions']"
    );
    roleHasPermissionElement.value = JSON.stringify(permissions);
    console.log(permissions);
}
function create() {
    beforeSubmit('createRoleForm');
    submitFormData('createRoleForm', [
        "input[name='Name']",
        "input[name='Permissions']",
    ]);
}
function update() {
    beforeSubmit('editRoleForm');
    submitFormData('editRoleForm', [
        "input[name='Name']",
        "input[name='Permissions']",
    ]);
}
function remove(id) {
    const formElement = document.getElementById('deleteRoleForm');
    const hiddenElement = formElement.querySelector('input[name="id"]');
    hiddenElement.value = id;
    submitFormData('deleteRoleForm', ["input[name='id']"]);
}

function edit(button) {
    var formElement = document.getElementById('editRoleForm');
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    ["input[name='id']", "input[name='Name']"].forEach((selector) => {
        var inputElement = formElement.querySelector(selector);
        var prop = inputElement.getAttribute('name');
        prop = prop.charAt(0).toLocaleLowerCase() + prop.slice(1);
        inputElement.value = entity[prop];
    });
    const url = formElement.action.replace('__id', entity.id);
    axios.get(url).then((response) => {
        var rolePermissions = response.data;
        var permissionElements = formElement.querySelectorAll(
            "input[name='Permission']"
        );
        permissionElements.forEach((permissionElement) => {
            var permissionId = permissionElement.value;
            var parentElement = permissionElement.parentElement;
            var readCheckElement =
                parentElement.querySelector("input[name='Read']");
            var writeCheckElement = parentElement.querySelector(
                "input[name='Write']"
            );
            var createCheckElement = parentElement.querySelector(
                "input[name='Create']"
            );
            var rolePermission = rolePermissions.find(
                (rp) => rp.permissionId == permissionId
            );
            if (rolePermission) {
                readCheckElement.checked = rolePermission.action.includes('r');
                writeCheckElement.checked = rolePermission.action.includes('w');
                createCheckElement.checked =
                    rolePermission.action.includes('c');
            } else {
                readCheckElement.checked = false;
                writeCheckElement.checked = false;
                createCheckElement.checked = false;
            }
        });
    });
}

export default {
    create,
    edit,
    update,
    remove,
};
