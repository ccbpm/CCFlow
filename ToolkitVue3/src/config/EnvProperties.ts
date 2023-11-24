declare global {
  interface Window {
    plant: string | undefined
  }
}
const {
  VITE_GLOB_CCFLOW_HANDLER,
  VITE_GLOB_JFLOW_HANDLER,
  VITE_GLOB_PLATFORM,
  VITE_GLOB_CCFLOW_UPLOAD_URL,
  VITE_GLOB_JFLOW_UPLOAD_URL
} = import.meta.env
let REQUEST_URL: any = ''
if (typeof window.plant === 'string') {
  REQUEST_URL =
    window.plant.toLowerCase() === 'ccflow' ? VITE_GLOB_CCFLOW_HANDLER : VITE_GLOB_JFLOW_HANDLER
} else {
  REQUEST_URL = VITE_GLOB_PLATFORM === 'CCFLOW' ? VITE_GLOB_CCFLOW_HANDLER : VITE_GLOB_JFLOW_HANDLER
}

const RICH_TEXT_URL = `/DataUser/CCForm/BigNoteHtmlText`
let REQUEST_UPLOAD_URL: any = ''
if (typeof window.plant === 'string') {
  REQUEST_UPLOAD_URL =
    window.plant.toLowerCase() === 'ccflow' ? VITE_GLOB_CCFLOW_HANDLER : VITE_GLOB_JFLOW_HANDLER
} else {
  REQUEST_UPLOAD_URL =
    VITE_GLOB_PLATFORM === 'CCFLOW' ? VITE_GLOB_CCFLOW_UPLOAD_URL : VITE_GLOB_JFLOW_UPLOAD_URL
}

export { REQUEST_URL, REQUEST_UPLOAD_URL, RICH_TEXT_URL }
