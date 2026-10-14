import "./adminPage_Users"

import useRequest from "../hooks/useRequest";

const roleNames: string[] = ["Default", "Admin"]

export default function () {
    const { data } = useRequest(api => api.admin_GetUsers());

    return (<table>
        <thead>
            <tr>
                <th>
                    Id
                </th>
                <th>
                    Name
                </th>
                <th>
                    Email
                </th>
                <th>
                    Role
                </th>
            </tr>
        </thead>
        <tbody>
            {
                data?.map(d => (
                    <tr>
                        <td>{d.id}</td>
                        <td>{d.displayName}</td>
                        <td>{d.email}</td>
                        <td>
                            <select value={d.role}>
                                {roleNames.map((r, i) => <option value={i} key={r}>{r}</option>)}
                            </select>
                        </td>
                    </tr>
                ))
            }
        </tbody>
    </table>)
}