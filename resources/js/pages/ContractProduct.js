import axios from 'axios';
import Swal from 'sweetalert2';

function upload(button) {
    const formElement = document.getElementById('importProductsForm');
    button.disabled = true;
    button.innerHTML =
        '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Uploading...';
    const url = formElement.action;
    const files = formElement.querySelector('input[name="files"]').files;
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
    }

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    ).value;
    axios
        .post(url, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
                RequestVerificationToken: token,
            },
        })
        .then((success) => {
            console.log('success :>> ', success);
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: success.data,
            }).then(() => {
                location.reload();
            });
        })
        .catch((error) => {
            console.log('error :>> ', error);
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: error.message,
            });
        });
}

export default { upload };
