export const defaultConnectStyle = {
    anchor: [
        "Left",
        "Right",
        "Top",
        "Bottom",
        [0.3, 0, 0, -1],
        [0.7, 0, 0, -1],
        [0.3, 1, 0, 1],
        [0.7, 1, 0, 1],
    ],
    connector: ["Flowchart", {stu: 30, cornerRadius: 10}],
    endpoint: "Blank",
    overlays: [["Arrow", {width: 8, length: 8, location: 1}], ["Label", {
        label: '',
        location: 0.1,
        cssClass: 'aLabel'
    }]], // overlay
    // 添加样式
    paintStyle: {stroke: "rgb(224, 227, 231)", strokeWidth: 1},
    hoverPaintStyle: {stroke: "#459dff", strokeWidth: 4}
};

export const connectOptions = {
    isSource: true,
    isTarget: true,
    // 动态锚点、提供了4个方向 Continuous、AutoDefault
    anchor: 'Continuous',
}