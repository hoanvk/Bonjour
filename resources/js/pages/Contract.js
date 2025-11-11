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
    Swal.fire({
        title: 'Confirm delete?',
        text: 'Are you sure you want to delete this contract?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, confirm it!',
    }).then((result) => {
        if (result.isConfirmed) {
            const formElement = document.getElementById('deleteContractForm');
            const hiddenElement = formElement.querySelector('input[name="id"]');
            hiddenElement.value = id;
            submitFormData('deleteContractForm', []);
        }
    });
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
