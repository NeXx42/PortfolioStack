<h1>Portfolio Stack</h1>
A simple portfolio site with a modern tech stack. Features a C# backend, PostgreSQL database, and a Next.js frontend, including JWT-based authentication.

<h1>Screenshots</h1>
<img width="300" alt="image" src="https://github.com/user-attachments/assets/83ed3cd4-5430-4b88-a0ea-c1c4e4b2389b" />
<img width="300" alt="image" src="https://github.com/user-attachments/assets/09d55f4d-b3bb-4acd-9465-8683f0208bef" />
<img width="300" alt="image" src="https://github.com/user-attachments/assets/59f3a92d-25c0-420d-8e72-c9c3f4018533" />
<img width="300" alt="image" src="https://github.com/user-attachments/assets/ffd682f0-884f-4bbd-9638-288446822701" />
<img width="600" height="927" alt="image" src="https://github.com/user-attachments/assets/c773d896-697e-431c-b636-4c014f7deeb5" />




<h1>Useful commands</h1>

Connecting to the server
```sh
sh -i ~/.ssh/PATHTOSSH matthew@SERVERIP
```

testing actions
```sh
act workflow_dispatch -j release -e .inputs.json -s .secrets
```
list actions
```sh
act --list
```


On the server

Verify nginx
```sh
sudo nginx -t

```

Restart nginx
```sh
sudo systemctl reload nginx
```
