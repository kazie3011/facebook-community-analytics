<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <style>
    .table {
        width: 100%;
        margin-bottom: 1rem;
        background-color: transparent;
        font-size:17px;
    }
    table {
        border-collapse: collapse;
    }
    .row-odd {
         background-color: rgba(0,0,0,.05);
    }

    .table td, .table th {
        padding: .75rem;
        vertical-align: top;
        border-top: 1px solid #dee2e6;
    }

    .text-info {
        color: #17a2b8!important;
    }

    p {
        margin-top: 0;
        margin-bottom: 1rem;
    }
    .w-50{
        width:50%
    }
    </style>
</head>
<body>
    <h1>A sample email sent from {{model.sender_name}} at {{model.created_at}}</h1>
</body>
</html>