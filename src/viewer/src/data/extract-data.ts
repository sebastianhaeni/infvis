/**
 * Transforms the extracted history into a hierarchy format.
 */
export function extractData(samples) {
    const data = {name: "Root", children: []};
    samples.forEach((sample) => {
        sample.Files
            .filter((file) => file.LinesAdded > 0)
            .map((file) => {
                const pathParts = file.FilePath.split("/");

                let current = data;

                pathParts.slice(0, pathParts.length - 1).forEach((part) => {
                    const existingChild = current.children.find((c) => c.name === part);

                    if (existingChild) {
                        current = existingChild;
                        return;
                    }

                    const newChild = {
                        children: existingChild && existingChild.children || [],
                        name: part,
                    };
                    current.children.push(newChild);
                    current = newChild;
                });

                current.children.push({
                    name: pathParts[pathParts.length - 1],
                    size: file.LinesAdded,
                });
            });
    });
    console.log(data);
    return data;
}
