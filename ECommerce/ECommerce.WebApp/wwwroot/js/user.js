var dataTable;

$(document).ready(function () {
  loadDataTable();
});

function loadDataTable() {
  dataTable = $("#tblData").DataTable({
    ajax: { url: "/admin/user/getall" },
    columns: [
      { data: "applicationUser.name", width: "15%" },
      { data: "applicationUser.email", width: "15%" },
      { data: "applicationUser.phoneNumber", width: "15%" },
      { data: "applicationUser.company.name", width: "15%" },
      { data: "role", width: "15%" },
      {
        data: "applicationUser",
        render: function (data) {
          var today = new Date().getTime();
          var lockout = new Date(data.lockoutEnd).getTime();

          if (lockout > today) {
            return `
                <div class="text-center">
                    <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer;">
                        <i class="bi bi-unlock-fill"></i> Unlock
                    </a>
                    <a href="/admin/user/rolemanagement?id=${data.id}" class="btn btn-danger text-white" style="cursor:pointer;">
                        <i class="bi bi-pencil-square"></i> Permission
                    </a>
                </div>
            `;
          } else {
            return `
                <div class="text-center">
                    <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer;">
                        <i class="bi bi-lock-fill"></i> Lock
                    </a>
                    <a href="/admin/user/rolemanagement?id=${data.id}" class="btn btn-danger text-white" style="cursor:pointer;">
                        <i class="bi bi-pencil-square"></i> Permission
                    </a>
                </div>
            `;
          }
        },
        width: "25%",
      },
    ],
  });
}

function LockUnlock(id) {
  $.ajax({
    type: "POST",
    url: "/Admin/User/LockUnlock",
    data: JSON.stringify(id),
    contentType: "application/json",
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);
        dataTable.ajax.reload();
      }
    },
  });
}

function GoToRoleManager(id) {
  $.ajax({
    type: "POST",
    url: "/Admin/User/LockUnlock",
    data: JSON.stringify(id),
    contentType: "application/json",
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);
        dataTable.ajax.reload();
      }
    },
  });
}
