import React from 'react'
import { useDataUpdate } from 'hookui-framework'
import * as styles from './styles'

const $Panel = ({ title, children, react }) => {

    const [maxHeight, setMaxHeight] = react.useState(0);
    react.useEffect(() => {
        const calculateMaxHeight = () => {
            // Calculate 50% of the window height
            const windowHeight = window.innerHeight;
            const newMaxHeight = windowHeight * 0.5;

            // Set the state with the new maxHeight value
            setMaxHeight(newMaxHeight);
        };

        // Calculate initial maxHeight on mount
        calculateMaxHeight();

        // Add event listener to update maxHeight on window resize
        window.addEventListener('resize', calculateMaxHeight);

        // Clean up the event listener on component unmount
        return () => {
            window.removeEventListener('resize', calculateMaxHeight);
        };
    }, []); // Empty dependency array ensures the effect runs only once on mount

    const [position, setPosition] = react.useState({ top: 100, left: 10 });
    const [dragging, setDragging] = react.useState(false);
    const [rel, setRel] = react.useState({ x: 0, y: 0 }); // Position relative to the cursor
    const [topValue, setTopValue] = react.useState(0);
    const panelStyle = { position: 'absolute', maxHeight: maxHeight + 'rem' };

    const [version, setVersion] = react.useState(null);
    useDataUpdate(react, 'extendedHotkeys.version', setVersion);

    const onMouseDown = (e) => {
        if (e.button !== 0) return; // Only left mouse button
        const panelElement = e.target.closest('.panel_YqS');

        // Calculate the initial relative position
        const rect = panelElement.getBoundingClientRect();
        setRel({
            x: e.clientX - rect.left,
            y: e.clientY - rect.top,
        });

        setDragging(true);
        e.stopPropagation();
        e.preventDefault();
    }

    const onMouseUp = (e) => {
        setDragging(false);
        // Remove window event listeners when the mouse is released
        window.removeEventListener('mousemove', onMouseMove);
        window.removeEventListener('mouseup', onMouseUp);
    }

    const onMouseMove = (e) => {
        if (!dragging) return;

        setPosition({
            top: e.clientY - rel.y,
            left: e.clientX - rel.x,
        });
        e.stopPropagation();
        e.preventDefault();
    }

    const onClose = () => {
        const data = { type: "toggle_visibility", id: '89pleasure.extendedHotkeys' };
        const event  = new CustomEvent('hookui', { detail: data });
        window.dispatchEvent(event);
    }

    const draggableStyle = {
        ...panelStyle,
        top: position.top + 'px',
        left: position.left + 'px',
    }

    const handleScroll = (event) => {
        setTopValue(event.target.scrollTop);
    }
    const scrollableStyle = { height: '200px', top: topValue };
    const closeStyle = { maskImage: 'url(Media/Glyphs/Close.svg)' };
    const versionString = 'v' + version;

    react.useEffect(() => {
        if (dragging) {
            // Attach event listeners to window
            window.addEventListener('mousemove', onMouseMove);
            window.addEventListener('mouseup', onMouseUp);
        }

        return () => {
            // Clean up event listeners when the component unmounts or dragging is finished
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
        };
    }, [dragging]); // Only re-run the effect if dragging state changes

    return (
        <div className="panel_YqS active-infoview-panel_aTq" style={draggableStyle}>
            <div className="header_H_U header_Bpo child-opacity-transition_nkS" onMouseDown={onMouseDown}>
                <div className="title-bar_PF4 title_Hfc">
                    <div className="icon-space_h_f">
                        {version && <div style={{ fontSize: 'var(--fontSizeXS)', color: 'rgba(255, 255, 255, 0.5)' }}>{versionString}</div>}
                    </div>
                    <div className="title_SVH title_zQN">{title}</div>
                    <button class="button_bvQ button_bvQ close-button_wKK" onClick={onClose}>
                        <div class="tinted-icon_iKo icon_PhD" style={closeStyle}></div>
                    </button>
                </div>
            </div>
            <div class="content_XD5 content_AD7 child-opacity-transition_nkS content_BIL"
                style={{ height: { maxHeight } + 'rem', overflowY: 'scroll', flexDirection: 'column' }}>
                <div class="section_sop section_gUk statistics-menu_y86" style={{ width: '100%' }}>
                    <div class="content_flM content_owQ first_l25 last_ZNw">
                        <div class="scrollable_DXr y_SMM track-visible-y_RCA scrollable_By7">
                            <div class="content_gqa" onScroll={handleScroll}>
                                <div className="content_Q1O">
                                    {children}
                                </div>
                                <div class="bottom-padding_JS3"></div>
                            </div>
                            <div class="track_e3O y_SMM">
                                <div id="scrollbar" class="thumb_Cib y_SMM" style={scrollableStyle}></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

const ExtendedHotkeysUI = ({ react }) => {

    // Translations
    const [translations, setTranslations] = react.useState({});
    useDataUpdate(react, 'extendedHotkeys.translations', setTranslations);

    // DISABLE MOD
    const [disableMod, setDisableMod] = react.useState(false);
    useDataUpdate(react, 'extendedHotkeys.disableMod', setDisableMod);

    const [enableNetToolWheel, setEnableNetToolWheel] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNetToolWheel', setEnableNetToolWheel);

    const [enableElevationWheel, setEnableElevationWheel] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableElevationWheel', setEnableElevationWheel);

    /*const [enableSnappingWheel, setSnappingWheel] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableSnappingWheel', setSnappingWheel);*/

    const [enableElevationReset, setEnableElevationReset] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableElevationReset', setEnableElevationReset);

    const [expandNTMGroup, setExpandNTMGroup] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.expandNTMGroup', setExpandNTMGroup);

    const [enableNTMGroup, setEnableNTMGroup] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.expandNTMGroup', setEnableNTMGroup);

    const [enableNTMStraight, setEnableNTMStraight] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNTMStraight', setEnableNTMStraight);

    const [enableNTMSimpleCurve, setEnableNTMSimpleCurve] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNTMSimpleCurve', setEnableNTMSimpleCurve);

    const [enableNTMComplexCurve, setEnableNTMComplexCurve] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNTMComplexCurve', setEnableNTMComplexCurve);

    const [enableNTMContinuous, setEnableNTMContinuous] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNTMContinuous', setEnableNTMContinuous);

    const [enableNTMGrid, setEnableNTMGrid] = react.useState(true);
    useDataUpdate(react, 'extendedHotkeys.enableNTMGrid', setEnableNTMGrid);

    const generalSettingsData = [
        { id: 0, label: translations['disableMod'], description: translations['disableMod.description'], isChecked: disableMod },
    ];

    const mouseWheelSettingsData = [
        { id: 1, label: "Tool Mode Wheel", description: "Scroll through NetTool modes.", isChecked: enableNetToolWheel, keyCode: 0 },
        { id: 2, label: "Elevation Wheel", description: "Increase/Decrease elevation.", expanded: true, isChecked: enableElevationWheel, keyCode: 2 },
        // { id: 3, label: "Snapping Wheel", description: "Switch through snapping templates.", isChecked: enableElevationWheel, keyCode: 1 },
    ];

    const staticHotkeysSettingsData = [
        {
            id: 11, label: "Anarchy Mode", description: "Toggle anarchy mode.", isChecked: true, hotkey: "CTRL + A",
        },
        {
            id: 4, label: "Elevation Reset", description: "Resets elevation to ground floor.", isChecked: enableElevationReset, hotkey: "POS1"
        },
        {
            id: 12, label: "Elevation Step Scroll", description: "Swap elevation step level.", isChecked: true, hotkey: "ALT + Mouse-R"
        },
        {
            id: 5, label: "Net Tool Modes", description: "Hop through net tool modes.", expanded: expandNTMGroup, isChecked: enableNTMGroup, children: [
                { id: 6, label: "Straight", description: translations['enableNTStraight.description'], isChecked: enableNTMStraight, hotkey: "CTRL + Q" },
                { id: 7, label: "Simple Curve", description: translations['enableNTSimpleCurve.description'], isChecked: enableNTMSimpleCurve, hotkey: "CTRL + W" },
                { id: 8, label: "Complex Curve", description: translations['enableNTComplexCurve.description'], isChecked: enableNTMComplexCurve, hotkey: "CTRL + E" },
                { id: 9, label: "Continuous", description: translations['enableNTContinuous.description'], isChecked: enableNTMContinuous, hotkey: "CTRL + R" },
                { id: 10, label: "Grid", description: translations['enableNTGrid.description'], isChecked: enableNTMGrid, hotkey: "CTRL + T" },
            ]
        },
    ];

    const Setting = ({ setting, nested }) => {
        const { label, isChecked, description, expanded, hotkey, keyCode, children } = setting;
        const checked_class = isChecked ? styles.CLASS_CHECKED : styles.CLASS_UNCHECKED

        const onToggle = () => {
            engine.trigger("extendedHotkeys.onToggle", setting.id);
        }

        const onExpand = () => {
            engine.trigger("extendedHotkeys.onExpand", setting.id);
        }

        // Available mouse wheel key codes
        const availableWheelKeyCodes = [ "SHIFT", "CTRL", "ALT"];

        const onExpandAction = children && children.length > 0 ? onExpand : null;
        const nestingStyle = { '--nesting': nested };
        const headerContentStyle = { marginTop: '-1rem' };
        const keyCodeStyle = { fontSize: 'var(--fontSizeS)', fontWeight: 'normal', textAlign: 'right', paddingRight: '10rem' };
        const decsriptionStyle = { fontSize: 'var(--fontSizeXS)' };
        const borderColor = isChecked ? 'rgba(134, 205, 144, 1.000000)' : 'rgba(134, 205, 144, 0.250000)';
        const borderStyle = {
            borderTopColor: borderColor,
            borderLeftColor: borderColor,
            borderRightColor: borderColor,
            borderBottomColor: borderColor
        };

        const maskImageStyle = { maskImage: expanded === false ? 'url(Media/Glyphs/ThickStrokeArrowRight.svg)' : 'url(Media/Glyphs/ThickStrokeArrowDown.svg)' }
        const renderChildren = () => {
            if (children && children.length > 0 && expanded) {
                return (
                    <div className="content_mJm foldout-expanded">
                        {children.map((child) => (
                            <Setting key={child.id} setting={child} onToggle={onToggle} nested={nested + 1} />
                        ))}
                    </div>
                );
            }

            return null;
        };

        const renderWheelKeyCodes = () => {
            if (keyCode !== undefined) {
                const label = availableWheelKeyCodes[keyCode] + " + Scroll";
                return (
                    <div className={styles.CLASS_TT_HEADER_CONTENT} style={keyCodeStyle}>
                        <div className={styles.CLASS_TT_LABEL}>{label}</div> 
                    </div>
                );
            }

            return null;
        };

        const renderHotkeys = () => {
            if (hotkey) {
                return (
                    <div className={styles.CLASS_TT_HEADER_CONTENT} style={keyCodeStyle}>
                        <div className={styles.CLASS_TT_LABEL}>{ hotkey }</div>
                    </div>
                );
            }

            return null;
        };

        return (
            <div className={styles.many(styles.CLASS_TT_FOLDOUT, styles.CLASS_TT_DISABLE_MOUSE_STATES)} style={nestingStyle}>
                <div className={styles.many(styles.CLASS_TT_HEADER, styles.CLASS_TT_ITEMMOUSESTATES, styles.CLASS_TT_ITEM_FOCUSED)}>
                    <div className={styles.CLASS_TT_ICON} onClick={onToggle}>
                        <div className={styles.many(styles.CLASS_TT_CHILD_TOGGLE, styles.CLASS_TT_ITEMMOUSESTATES, checked_class)} style={borderStyle}>
                            <div className={styles.many(styles.CLASS_TT_CHECKMARK, checked_class)}></div>
                        </div>
                    </div>
                    <div className={styles.CLASS_TT_HEADER_CONTENT} style={headerContentStyle} onClick={onExpandAction}>
                        <div className={styles.CLASS_TT_LABEL}>{label}</div>
                        {description && <div style={decsriptionStyle}>{description}</div>}
                    </div>
                    {renderWheelKeyCodes()}
                    {renderHotkeys()}
                    {children && children.length > 0 && <div class="tinted-icon_iKo toggle_RV4 toggle_yQv" style={maskImageStyle} onClick={onExpandAction}></div>}
                </div>
                {renderChildren()}
            </div>
        );
    }

    const SettingsList = ({ name, description, settings }) => {
        const decsriptionStyle = {
            fontSize: 'var(--fontSizeS)',
            fontWeight: 'normal',
        };
        
        return (
            <div class="statistics-category-item_qVI">
                <div class="header_Ld7">{name}</div>
                {description && <div className={styles.CLASS_TT_HEADER} style={decsriptionStyle}>{description}</div>}
                <div class="items_AIY">
                    {settings.map((setting) => (
                        <Setting
                            key={setting.id}
                            nested={0}
                            setting={setting}
                        />
                    ))}
                </div>
            </div>
        );
    };

    return <$Panel title="Extended Hotkeys" react={react}>
        <SettingsList name="General" settings={generalSettingsData} />
        <SettingsList name="Mouse Wheel" description="Extended actions with your mouse wheel while in net tool placing roads." settings={mouseWheelSettingsData} />
        <SettingsList name="Hotkeys" description="Awesome actions with your keyboard." settings={staticHotkeysSettingsData} />
    </$Panel>
};

window._$hookui.registerPanel({
    id: '89pleasure.extendedHotkeys',
    name: 'ExtendedHotkeys',
    icon: 'Media/Game/Icons/Journal.svg',
    component: ExtendedHotkeysUI
})