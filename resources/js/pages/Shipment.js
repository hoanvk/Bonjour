import submitFormData from '../submit-formdata';

function create() {
    submitFormData('createShipmentForm', [
        "input[name='carrier']",
        "input[name='consignee']",
        "input[name='departure']",
    ]);
}

function edit(button) {
    const entity = JSON.parse(button.getAttribute('data-bs-entity'));
    var formElement = document.getElementById('editShipmentForm');
    console.log(entity);
    [
        "input[name='id']",
        "input[name='carrier']",
        "input[name='consignee']",
        "input[name='departure']",
    ].forEach((selector) => {
        var inputElement = formElement.querySelector(selector);
        var prop = inputElement.getAttribute('name');
        prop = prop.charAt(0).toLocaleLowerCase() + prop.slice(1);
        inputElement.value = entity[prop];
    });
}

function update() {
    submitFormData('editShipmentForm', [
        "input[name='carrier']",
        "input[name='consignee']",
        "input[name='departure']",
    ]);
}

function remove(id) {
    Swal.fire({
        title: 'Confirm delete?',
        text: 'Are you sure you want to delete this shipment?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, confirm it!',
    }).then((result) => {
        if (result.isConfirmed) {
            const form = document.getElementById('deleteShipmentForm');
            const shipmentIdElement = form.querySelector('input[name="id"]');
            shipmentIdElement.value = id;
            submitFormData('deleteShipmentForm', []);
        }
    });
}

export default {
    create,
    edit,
    update,
    remove,
};
