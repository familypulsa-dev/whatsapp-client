import { useLocation } from "react-router-dom";
import TemplatePreview from "@/components/Templates/TemplatePreview";
import TemplateEditor from "@/components/Templates/TemplateEditor";
import TemplateCreate from "@/components/Templates/TemplateCreate";

export default function Template() {
    const location = useLocation();

    const isCreateMode = location.pathname.includes('/create');
    const isEditMode = location.pathname.includes('/edit/');
    const isPreviewMode = location.pathname.includes('/preview/');

    if (isCreateMode) {
        return <TemplateCreate />
    }

    if (isEditMode) {
        return <TemplateEditor />
    }

    if (isPreviewMode) {
        return <TemplatePreview />
    }

    return null;
}