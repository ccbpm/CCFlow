import { Result, Button } from 'antd'
import { useNavigate } from 'react-router-dom';

function Error404() {
  const navigate = useNavigate();

  return (
    <Result
      status="404"
      title="404"
      subTitle="您访问的页面不存在"
      extra={<Button type="primary" onClick={() => navigate('/wf/todo', { replace: true })}>
        返回待办
      </Button>}
    ></Result>
  )
}



export default Error404;