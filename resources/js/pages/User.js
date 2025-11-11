import submitFormData from '../submit-formdata';
import Swal from 'sweetalert2';

function create() {
    const formElement = document.getElementById('createUserForm');
    const rolesSelectElement = formElement.querySelector(
        'select[name="RoleId"]'
    );
    const roles = [];

    Array.from(rolesSelectElement.options).forEach((option) => {
        if (option.selected) {
            roles.push(option.value);
        }
    });
    const rolesInputElement = formElement.querySelector('input[name="Roles"]');
    rolesInputElement.value = JSON.stringify(roles);
    submitFormData('createUserForm', [
        'input[name="Name"]',
        'input[name="Username"]',
        'input[name="Password"]',
        'input[name="ConfirmPassword"]',
        'input[name="Roles"]',
    ]);
}

function remove(id) {
    Swal.fire({
        title: 'Confirm delete?',
        text: 'Are you sure you want to delete this user?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, confirm it!',
    }).then((result) => {
        if (result.isConfirmed) {
            const formElement = document.getElementById('deleteUserForm');
            const userIdElement = formElement.querySelector('input[name="id"]');
            userIdElement.value = id;
            submitFormData('deleteUserForm', []);
        }
    });
}

function edit(button) {
    const formElement = document.getElementById('editUserForm');
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    console.log(entity);
    [
        'input[name="id"]',
        'input[name="name"]',
        'input[name="username"]',
    ].forEach((selector) => {
        var inputElement = formElement.querySelector(selector);
        console.log(inputElement.getAttribute('name'));
        var prop = inputElement.getAttribute('name');
        inputElement.value = entity[prop];
    });

    const url = formElement.action.replace('__id', entity.id);
    console.log(url);
    axios.get(url).then((response) => {
        const roles = response.data.roles;
        const selectElement = formElement.querySelector(
            'select[name="RoleId"]'
        );
        for (let i = 0; i < selectElement.options.length; i++) {
            const option = selectElement.options[i];
            option.selected = roles.includes(parseInt(option.value));
        }
    });
}

function update() {
    const formElement = document.getElementById('editUserForm');
    const rolesSelectElement = formElement.querySelector(
        'select[name="RoleId"]'
    );
    const roles = [];

    Array.from(rolesSelectElement.options).forEach((option) => {
        if (option.selected) {
            roles.push(option.value);
        }
    });
    const rolesInputElement = formElement.querySelector('input[name="Roles"]');
    rolesInputElement.value = JSON.stringify(roles);
    submitFormData('editUserForm', [
        'input[name="name"]',
        'input[name="username"]',
        'input[name="Roles"]',
    ]);
}

function changePassword(button) {
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    const formElement = document.getElementById('changePasswordForm');
    const inputElement = formElement.querySelector('input[name="id"]');
    inputElement.value = entity.id;
}

function saveChangePassword(id) {
    submitFormData('changePasswordForm', [
        'input[name="password"]',
        'input[name="confirmPassword"]',
    ]);
}

export default {
    create,
    remove,
    edit,
    update,
    changePassword,
    saveChangePassword,
};
