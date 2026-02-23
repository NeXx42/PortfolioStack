Useful commands

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